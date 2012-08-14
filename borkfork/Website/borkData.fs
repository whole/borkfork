// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help
namespace borkData
module Connections  =
    #if INTERACTIVE
    #r "FSharp.Data.TypeProviders"
    #r  "System.Linq.dll"
    #r  "System.Data.Linq.dll"
    #endif


    open System
    open System.IO
    open System.Data
    open System.Data.Linq
    open Microsoft.FSharp.Data.TypeProviders

    // You can use Server Explorer to build your ConnectionString.Data Source=IRON;Initial Catalog=borkdorkfork;Integrated Security=True
    type  internal dbSchema = Microsoft.FSharp.Data.TypeProviders.SqlDataConnection<ConnectionString = @"Data Source=IRON;Initial Catalog=borkdorkfork;Integrated Security=True">
    let internal db = dbSchema.GetDataContext()

    // catching category_id and category_link from dataBase
    let internal category = seq {
        for r in db.CATEGORY do
        yield (r.Id, r.Link)
        } 


        // bloody recursive dependency grrrr
    let http (url: string) = 
        let req = System.Net.WebRequest.Create(url) 
        let resp = req.GetResponse() 
        let stream = resp.GetResponseStream() 
        let reader = new StreamReader(stream) 
        let html = reader.ReadToEnd() 
        resp.Close() 
        html 


    let rawData() = seq { 
        for oneCategory in category do
        yield (fst oneCategory, http (snd oneCategory))
         } 


    let  pushRawData() =
        let items = rawData()
        for i in items do
             db.RAWDATA.InsertOnSubmit( new dbSchema.ServiceTypes.RAWDATA(

                                           
                                            Time_of_scraping = System.DateTime.Now,
                                            Category_id      = fst i,
                                            Raw_data         = snd i))
             try
                db.DataContext.SubmitChanges()
                printfn "Successfully inserted new rows."
             with
               | exn -> printfn "Exception:\n%s" exn.Message


    let pullRawData() =  seq {
        for r in db.RAWDATA do
        yield (r.Category_id, r.Raw_data)
        } 