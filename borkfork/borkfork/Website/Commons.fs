namespace Commons
open System.IO
open System.Text.RegularExpressions

open borkData


module Parsing =
        // catching html of page
    let http (url: string) = 
        let req = System.Net.WebRequest.Create(url) 
        let resp = req.GetResponse() 
        let stream = resp.GetResponseStream() 
        let reader = new StreamReader(stream) 
        let html = reader.ReadToEnd() 
        resp.Close() 
        html 

module Timer = 
        let getTimes = 
            [
                for r in Connections.db.TIMES ->
                    (r.Id, r.TimeOfScraping)
            ]
        let calculateTime(s1: System.DateTime, s2:System.DateTime) : int  =
            System.Math.Abs(s2.Hour - s1.Hour)

 
        let updateTime() =    
          
             let x = Connections.db.TIMES.InsertOnSubmit( new Connections.dbSchema.ServiceTypes.TIMES (TimeOfScraping = System.DateTime.Now))
             try
                Connections.db.DataContext.SubmitChanges()
                printfn "Successfully inserted new rows."
             with
               | exn -> printfn "Exception:\n%s" exn.Message

 module Others =
    // this sucks but I am too lazy to lazy to do it.
        // generalise in the future
    let beatufyString (word:string) = 
      let newWord = word.Split(',')
      Regex.Replace(newWord.[0], @"\s", "")
   
