
@Echo OFF

If Exist "C:\Users\provider.bnogueira\" (
	If Exist "\\svdatfs01\Sistemas\" (
		if Not Exist "\\svdatfs01\Sistemas\bNogueira\AppUtil\AutoFillTS\Model\" MkDir "\\svdatfs01\Sistemas\bNogueira\AppUtil\AutoFillTS\Model"
		Copy /Y "D:\Prj\SVN\MPSC.AutoFillTS\Bin\*.dll" "\\svdatfs01\Sistemas\bNogueira\AppUtil\AutoFillTS\*.*"
		Copy /Y "D:\Prj\SVN\MPSC.AutoFillTS\Bin\MPSC.AutoFillTS.exe" "\\svdatfs01\Sistemas\bNogueira\AppUtil\AutoFillTS\*.*"
		Copy /Y "D:\Prj\SVN\MPSC.AutoFillTS\Bin\Model\Modelo.xlsx" "\\svdatfs01\Sistemas\bNogueira\AppUtil\AutoFillTS\*.*"
	)
)
