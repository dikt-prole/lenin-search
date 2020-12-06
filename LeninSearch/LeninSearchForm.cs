using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using LeninSearch.Core;
using Newtonsoft.Json;

namespace LeninSearch
{
    public partial class LeninSearchForm : Form
    {
        private static readonly Random Random = new Random();

        private static readonly string CurrentFolder = Path.Combine(Path.GetTempPath(), "LeninSearch_EE354AE8");

        private const string Placeholder = "Введите запрос и нажмите Enter";

        private readonly ToolTip _toolTip = new ToolTip();

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);

        public LeninSearchForm()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon("lenin.ico");

            corpus_cb.Items.Clear();

            var corpusItems = JsonConvert.DeserializeObject<List<CorpusItem>>(File.ReadAllText("corpus/corpus.json"));
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

            Shown += (sender, args) => query_tb.Focus();
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

            Text = "Lenin Search - Распаковка ...";

            PlayStart();

            if (Directory.Exists(CurrentFolder))
            {
                Directory.Delete(CurrentFolder, true);
            }
            Directory.CreateDirectory(CurrentFolder);

            var corpusItem = (CorpusItem)corpus_cb.SelectedItem;
            await Task.Run(() => FileUtil.UnzipCorpus(CurrentFolder, corpusItem));

            Text = "Lenin Search - Поиск ...";

            var files = Directory.GetFiles(CurrentFolder, "*.json").ToList();
            progressBar1.Minimum = 1;
            progressBar1.Value = 1;
            progressBar1.Maximum = files.Count;

            var searchSplit = query_tb.Text.Split('+');

            var searchOptions = new SearchOptions
            {
                SearchText = searchSplit[0].Trim(' '),
                AdditionalText = searchSplit.Length > 1 ? searchSplit.Last().Trim(' ') : null
            };

            var totalMatches = 0;

            for (var i = 0; i < files.Count; i++)
            {
                var file = files[i];

                var currentSearchOptions = searchOptions.Copy();
                currentSearchOptions.File = file;
                currentSearchOptions.Corpus = FileUtil.GetFileNameWithoutExtension(file);

                var searchResult = await Task.Run(() => Search(currentSearchOptions));

                if (searchResult.ParagraphIndexes.Count > 0)
                {
                    totalMatches += searchResult.ParagraphIndexes.Count;
                    PlayRandomFound();
                }

                AddLinkLabel(searchResult);

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
            var tuple = result_tv.SelectedNode?.Tag as Tuple<SearchResult, int>;
            if (tuple == null) return;



            var paragraphIndex = tuple.Item2;
            var fileData = tuple.Item1.FileData;

            result_rtb.Text = fileData.GetText(paragraphIndex, (int)bottomContext_nud.Value, (int)topContext_nud.Value);

            MarkText(tuple.Item1.SearchOptions.SearchText);
            if (!string.IsNullOrWhiteSpace(tuple.Item1.SearchOptions.AdditionalText))
            {
                var words = TextUtil.GetIndexWords(tuple.Item1.SearchOptions.AdditionalText);
                foreach (var word in words)
                {
                    MarkText(word);
                }
            }
        }

        private void MarkText(string text)
        {
            var selectionLength = text.Length;
            var selectionStart = result_rtb.Text.ToLower().IndexOf(text.ToLower());
            while (selectionStart >= 0)
            {
                result_rtb.Select(selectionStart, selectionLength);
                result_rtb.SelectionFont = new Font(result_rtb.Font, FontStyle.Bold);
                selectionStart = result_rtb.Text.ToLower().IndexOf(text.ToLower(), selectionStart + selectionLength);
            }
        }

        private SearchResult Search(SearchOptions searchOptions)
        {
            try
            {
                var fileDataJson = File.ReadAllText(searchOptions.File);
                var fileData = JsonConvert.DeserializeObject<FileData>(fileDataJson);
                var searchWords = TextUtil.GetIndexWords(searchOptions.SearchText);
                if (!string.IsNullOrEmpty(searchOptions.AdditionalText))
                {
                    searchWords = searchWords.Concat(TextUtil.GetIndexWords(searchOptions.AdditionalText)).Distinct().ToList();
                }

                List<int> suspectIndexes = null;
                foreach (var searchWord in searchWords)
                {
                    var searchWordIndexes = fileData.Index.Keys.Where(k => k.StartsWith(searchWord)).SelectMany(k => fileData.Index[k]).Distinct();
                    suspectIndexes = suspectIndexes == null
                        ? searchWordIndexes.ToList()
                        : suspectIndexes.Intersect(searchWordIndexes).ToList();
                }

                suspectIndexes = suspectIndexes.OrderBy(x => x).ToList();

                var paragraphIndexes = new List<int>();
                foreach (var suspectIndex in suspectIndexes)
                {
                    var text = fileData.Paragraphs[suspectIndex];

                    if (text == null) continue;

                    if (!text.ToLower().Contains(searchOptions.SearchText.ToLower())) continue;

                    paragraphIndexes.Add(suspectIndex);
                }

                return new SearchResult
                {
                    Id = Guid.NewGuid(),
                    SearchOptions = searchOptions,
                    Success = true,
                    ErrorMessage = null,
                    ParagraphIndexes = paragraphIndexes,
                    FileData = fileData
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

        private void AddLinkLabel(SearchResult searchResult)
        {
            if (searchResult.Success && searchResult.ParagraphIndexes.Any())
            {
                var treeNode = new TreeNode(searchResult.ToString());
                result_tv.Nodes.Add(treeNode);
                foreach (var paragraphIndex in searchResult.ParagraphIndexes)
                {
                    var nodeText = new string(searchResult.FileData.Paragraphs[paragraphIndex].Take(20).ToArray()) + " ...";
                    var paragraphNode = new TreeNode(nodeText);
                    treeNode.Nodes.Add(paragraphNode);
                    paragraphNode.Tag = new Tuple<SearchResult, int>(searchResult, paragraphIndex);
                }
            }
        }
    }
}
