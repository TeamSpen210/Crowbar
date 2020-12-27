Imports System.IO
Imports Fclp
Public Module CommandLine
	Public Enum ParseResult
		GUI = 0
		Success = 1
		Err = 2
	End Enum
	Public Function Run(argv As String()) As ParseResult
		Console.Out.WriteLine("Crowbar Version " + My.Application.Info.Version.ToString(2))
		Dim command As String = argv(0).ToLowerInvariant()
		Dim params As String() = New String(){}
		Array.Resize(params, argv.Length - 1)
		Array.Copy(argv, 1, params, 0, argv.Length - 1)
		
		Dim parser As FluentCommandLineParser = New FluentCommandLineParser()
		Dim settings As AppSettings = TheApp.Settings
		If command = "decompile" Then
			TheApp.IsCommandLine = True
			settings.DecompileMode = InputOptions.File

			Dim inputMDl As String = ""
			Dim outputFolder As String = ""
			parser.Setup(Of String)("i"c, "input").Callback(Sub(value As String) inputMDl = value).Required()
			parser.Setup(Of String)("o"c, "output").Callback(Sub(value As String) outputFolder = value).Required()
			parser.Setup(Of Boolean)("folder").SetDefault(False).Callback(Sub(value) settings.DecompileFolderForEachModelIsChecked = value)
			parser.Setup(Of Boolean)("no-ref").SetDefault(False).Callback(Sub(value) settings.DecompileReferenceMeshSmdFileIsChecked = Not value)
			parser.Setup(Of Boolean)("no-phy").SetDefault(False).Callback(Sub(value) settings.DecompilePhysicsMeshSmdFileIsChecked = Not value)
			parser.Setup(Of SupportedMdlVersion)("version").SetDefault(SupportedMdlVersion.DoNotOverride).Callback(Sub(value) settings.DecompileOverrideMdlVersion = value)
			parser.Setup(Of Boolean)("strict-format").SetDefault(False).Callback(Sub(value) settings.DecompileStricterFormatIsChecked = value)

			Dim result As ICommandLineParserResult = parser.Parse(params)
			if result.HasErrors then
				Console.Out.WriteLine(result.ErrorText)
				Return ParseResult.Err
			End If
			TheApp.Decompiler.DecompileCommandLine(inputMDL, outputFolder, Console.Out)
		ElseIf command.StartsWith(App.SettingsParameter) Then
			Return ParseResult.GUI
		Else 
			Console.Out.WriteLine("Unknown command " + command + "!")
			Return ParseResult.Err
		End If
		Return ParseResult.Success
	End Function
End Module
