SET "Src=D:\Repo\lenin-search\corpus\json\MarxEngels"
SET "Str=marx-engels-"
for %%a in ("%Src%\*.*") do ren "%%~a" "%Str%%%~Na%%~Xa"