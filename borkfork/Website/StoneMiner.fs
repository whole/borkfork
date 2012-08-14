namespace StoneMiner
open borkData
open Commons

module FilterJobs =
       let getItems() = seq { for i in Connections.db.ONEPOST do
                                yield [i.Title; i.Link; i.Poslodavac; i.Mjesto_rada; i.Rok_za_prijavu] 
                          } 
                            // we remove duplicates... I don't know how expensive
                          //this operation is. I will calculate it eventually.
                                
       let items = getItems()
       let filterItems() = Set.ofSeq(items) 

//       let pushPostContent = seq {
//                for i in filterItems do
//                    yield [i.[1]; i.[1] |> Parsing.http; ]  }
//                    
//        
//       let pushToDb (s: seq<string list>) = 
//             for i in s do
//                Connections.db.POSTRAWDATA.InsertOnSubmit( new Connections.dbSchema.ServiceTypes.POSTRAWDATA
//                    (
//                      Link         = i.[0],
//                      RawData      = i.[1],
//                      Id_of_post   = 2L
//                      )
//                    )
//                try
//                    Connections.db.DataContext.SubmitChanges()
//                    printfn "Successfully inserted new rows."
//                with
//                    | exn -> printfn "Exception:\n%s" exn.Message
//
