open FSharp.Json
open System.IO
open System
open System.Reflection
open System.Text.RegularExpressions


type Cell =
    { source : string list
    }
type Notebook =
    { cells : Cell list
    }

type SnippetData =
    { body : string list
      prefix : string
      description : string
    }
type Snippets =
    Map<string, SnippetData>

[<EntryPoint>]
let main args =
    let inPath = args[0]
    let outPath = args[1]
    let content = File.ReadAllText(args[0])
    printfn $"{content}"
    let nb = Json.deserialize<Notebook> content
    printfn $"{nb}"
    let snippets =
        List.fold
            (fun (state : Snippets) x ->
                let prefix = x.source[0].TrimStart([|'#'; ' '|]).Trim()
                state.Add(
                    prefix,
                    {
                        body =
                            (x.source[1..] |> List.map (fun x -> Regex.Replace(x.Trim(), @"ö(\d+)_([^ö]*)ö", "$" + "{$1:$2}")))
                            @ ["$0"]
                        prefix = prefix
                        description = ""
                    })
            )
            Map.empty
            nb.cells
    printfn $"{snippets}"
    File.WriteAllText(outPath, snippets |> Json.serialize)
    0





