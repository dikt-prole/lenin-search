using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LeninSearch.Core;
using LeninSearch.Core.Oprimized;
using Newtonsoft.Json;

namespace LeninSearch
{
    public partial class LeninSearchForm : Form
    {
        private static readonly Random Random = new Random();

        private static readonly string CurrentFolder = Path.Combine(Path.GetTempPath(), "LeninSearch_EE354AE8");

        private const string Placeholder = "Введите запрос и нажмите Enter";

        private readonly ToolTip _toolTip = new ToolTip();

        private const string Loading = "Загрузка ...";

        // is needed to reconstruct paragraph text
        private string[] _dictionary = null;

        // is needed to get word indexes for search query
        private Dictionary<string, uint> _reverseDictionary = null; 

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        public LeninSearchForm()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon("lenin.ico");

            bottomContext_nud.Minimum = 0;
            bottomContext_nud.Maximum = 1000;
            bottomContext_nud.Value = 3;
            topContext_nud.Minimum = 0;
            topContext_nud.Maximum = 1000;
            topContext_nud.Value = 3;

            query_tb.PlaceholderText = Placeholder;
            query_tb.KeyPress += SearchQuery_tbOnKeyPress;
            result_tv.AfterSelect += (sender, args) => RefreshResultTb();
            bottomContext_nud.ValueChanged += (sender, args) => RefreshResultTb();
            topContext_nud.ValueChanged += (sender, args) => RefreshResultTb();

            corpus_cb.Items.Add(Loading);
            corpus_cb.SelectedItem = Loading;
            corpus_cb.Enabled = false;
            corpus_cb.KeyPress += (sender, args) => args.Handled = true;

            query_tb.Enabled = false;
            Shown += OnShown;
        }

        private async void OnShown(object? sender, EventArgs e)
        {
            var corpusJson = File.ReadAllText($"corpus.json");
            var corpusItems = JsonConvert.DeserializeObject<List<CorpusItem>>(corpusJson);

            _dictionary = (await File.ReadAllLinesAsync($"{CurrentFolder}\\ls\\corpus.dic")).Where(s => s != "").ToArray();
            _reverseDictionary = new Dictionary<string, uint>();
            for (uint i = 0; i < _dictionary.Length; i++)
            {
                _reverseDictionary.Add(_dictionary[i], i);
            }

            corpus_cb.Items.Clear();
            foreach (var ci in corpusItems)
            {
                corpus_cb.Items.Add(ci);
                if (ci.Selected)
                {
                    corpus_cb.SelectedItem = ci;
                }
            }
            corpus_cb.MouseEnter += (sender, args) =>
            {
                var posistion = new Point(15, 65);
                var text = (corpus_cb.SelectedItem as CorpusItem).Description;
                _toolTip.Show(text, this, posistion, 2000);
            };

            var tempCorpusJsonPath = $"{CurrentFolder}\\ls\\corpus.json";
            if (!File.Exists(tempCorpusJsonPath) || CryptoUtil.GetMD5(tempCorpusJsonPath) != CryptoUtil.GetMD5("corpus.json"))
            {
                await Task.Run(() =>
                {
                    if (Directory.Exists(CurrentFolder))
                    {
                        Directory.Delete(CurrentFolder, true);
                        Directory.CreateDirectory(CurrentFolder);
                    }
                    ZipFile.ExtractToDirectory("corpus.zip", CurrentFolder);
                });
            }

            corpus_cb.Enabled = true;
            query_tb.Enabled = true;
            query_tb.Focus();
        }

        private async void SearchQuery_tbOnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Return) return;

            e.Handled = true;

            if (string.IsNullOrWhiteSpace(query_tb.Text)) return;

            Invoke(new Action(() =>
            {
                SendMessage(progressBar1.Handle, 1040, 2, 0);
            }));

            result_tv.Nodes.Clear();

            PlayStart();

            var corpusItem = (CorpusItem)corpus_cb.SelectedItem;

            Text = "Lenin Search - Поиск ...";

            progressBar1.Minimum = 1;
            progressBar1.Value = 1;
            progressBar1.Maximum = corpusItem.Files.Count;

            var searchOptions = new SearchOptions(query_tb.Text, "", "", _reverseDictionary);

            var totalMatches = 0;

            for (var i = 0; i < corpusItem.Files.Count; i++)
            {
                var corpusFileItem = corpusItem.Files[i];

                var currentSearchOptions = searchOptions.Copy();
                currentSearchOptions.File = $"{CurrentFolder}\\ls\\{corpusFileItem.Path}";
                currentSearchOptions.Corpus = corpusFileItem.Name;

                var fileData = ArchiveUtil.LoadOptimized($"{CurrentFolder}\\ls\\{corpusFileItem.Path}", CancellationToken.None);

                var searchResult = await Task.Run(() => Search(fileData, currentSearchOptions));

                if (searchResult.OptimizedParagraphs.Count > 0)
                {
                    totalMatches += searchResult.OptimizedParagraphs.Count;
                    PlayRandomFound();
                }

                OutputSearchResult(searchResult);

                if (i + 2 <= progressBar1.Maximum)
                {
                    progressBar1.Value = i + 2;
                    progressBar1.Value = i + 1;
                }

                Text = $"Lenin Search ... {totalMatches} совпадений";
            }

            PlayStop();

            Text = $"Lenin Search -  найдено {totalMatches} совпадений";

            if (result_tv.Nodes.Count > 0)
            {
                result_tv.Nodes[0].Expand();
                result_tv.SelectedNode = result_tv.Nodes[0].Nodes[0];
            }
        }

        private void RefreshResultTb()
        {
            var tuple = result_tv.SelectedNode?.Tag as Tuple<SearchResult, OptimizedParagraph>;
            if (tuple == null) return;

            var paragraph = tuple.Item2;
            var ofd = tuple.Item1.OptimizedFileData;

            var paragraphs = ofd.GetParagraphs(paragraph.Index, (ushort)topContext_nud.Value, (ushort)bottomContext_nud.Value, _dictionary);

            var paragraphsText = string.Join($"{Environment.NewLine}{Environment.NewLine}", paragraphs.Select(p => p.GetText(_dictionary)));

            var header = ofd.GetHeader(paragraph.Index);

            var page = ofd.GetPage(paragraph.Index);

            if (page != null || header != null)
            {
                var pageHeader = page == null
                    ? header.GetText(_dictionary)
                    : header == null
                        ? $"стр. {page}"
                        : $"стр. {page}, {header.GetText(_dictionary)}";
                paragraphsText = $"{pageHeader + Environment.NewLine}{string.Join("", Enumerable.Repeat("-", 30))}{Environment.NewLine + Environment.NewLine}{paragraphsText}";
            }

            result_rtb.Text = paragraphsText;

            MarkText(tuple.Item1.SearchOptions.MainQuery);
            if (!string.IsNullOrWhiteSpace(tuple.Item1.SearchOptions.AdditionalQuery))
            {
                var words = TextUtil.GetIndexWords(tuple.Item1.SearchOptions.AdditionalQuery);
                foreach (var word in words)
                {
                    MarkText(word);
                }
            }
        }

        private void MarkText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            var selectionLength = text.Length;
            var selectionStart = result_rtb.Text.ToLower().IndexOf(text.ToLower());
            while (selectionStart >= 0)
            {
                result_rtb.Select(selectionStart, selectionLength);
                result_rtb.SelectionFont = new Font(result_rtb.Font, FontStyle.Bold);
                selectionStart = result_rtb.Text.ToLower().IndexOf(text.ToLower(), selectionStart + selectionLength);
            }
        }

        private SearchResult Search(OptimizedFileData ofd, SearchOptions searchOptions)
        {
            try
            {
                var paragraphs = ofd.FindParagraphs(searchOptions, _dictionary).ToList();
                return new SearchResult
                {
                    Id = Guid.NewGuid(),
                    SearchOptions = searchOptions,
                    Success = true,
                    ErrorMessage = null,
                    OptimizedParagraphs = paragraphs,
                    OptimizedFileData = ofd
                };
            }
            catch (Exception exc)
            {
                return new SearchResult
                {
                    Id = Guid.NewGuid(),
                    SearchOptions = searchOptions,
                    Success = false,
                    ErrorMessage = exc.ToString()
                };
            }
        }

        private void PlayRandomFound()
        {
            if (Random.Next(2) == 1)
            {
                Sounds.Play(Sounds.SomeMarks);
            }
            else
            {
                Sounds.Play(Sounds.YesAlmost);
            }
        }

        private void PlayStart()
        {
            Sounds.Play(Sounds.LetMe, true);
        }

        private void PlayStop()
        {
            Sounds.Play(Sounds.So, true);
        }

        private void OutputSearchResult(SearchResult searchResult)
        {
            if (searchResult.Success && searchResult.OptimizedParagraphs.Any())
            {
                var treeNode = new TreeNode(searchResult.ToString());
                result_tv.Nodes.Add(treeNode);
                foreach (var p in searchResult.OptimizedParagraphs)
                {
                    

                    var nodeText = new string(p.GetText(_dictionary).Take(20).ToArray()) + " ...";
                    var paragraphNode = new TreeNode(nodeText);
                    treeNode.Nodes.Add(paragraphNode);
                    paragraphNode.Tag = new Tuple<SearchResult, OptimizedParagraph>(searchResult, p);
                }
            }
        }
    }
}
