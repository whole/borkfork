namespace Parsing 

open System.Collections

open System.Xml
open System.Linq
open System.Xml.Linq

open borkData

module ParsePosaohr =
   
    let xn s = XName.Get(s)

    let main rawData = 
        let xd = XDocument.Parse(snd (rawData))
        //When using XLinq in F#, you need a simple utility function for converting strings to XName object (which represents an element/attribute name). There is an implicit conversion in C#, but this sadly doesn't work in F#.
        
        // we will get IEnumerable of all items.
        let items = xd.Element( System.Xml.Linq.XName.Get("rss")).Element(System.Xml.Linq.XName.Get("channel")).Elements(System.Xml.Linq.XName.Get("item"))
        ((fst rawData), items.Take 10) 
        // after this we will use our type singlePost as temp record before saving data

             (* 
                DAKLE TREBAMO DESCRIPTION SPLITAT NA OBJEKTE: primjer.
                value = Description = i.Element(xn "description").Value 
                primjer:   "Poslodavac: CERP - Centar za racunalnu podršku
                <div>Mjesto rada: Zagreb</div><div>Rok za prijavu: 15.08.2012.</div>"
             *)
    open System.Text.RegularExpressions
    let splitDescription (s: string) = 
        let splitedString = Regex.Split( s.Replace("</div>", ""), @"<div>")
        [ (splitedString.[0].Split(':')).[1];
          (splitedString.[1].Split(':')).[1]; 
           (splitedString.[2].Split(':')).[1];]

    
    let returnToken (items: int64 * Generic.IEnumerable<XElement>) = 
          let getItems = snd items
          for i in getItems do
                     Connections.db.ONEPOST.InsertOnSubmit( new Connections.dbSchema.ServiceTypes.ONEPOST
                        (
                            Title =   i.Element(xn "title").Value,
                            Link  = i.Element(xn "link").Value,
                            Poslodavac   = (splitDescription (i.Element(xn "description").Value)).[0],
                            Mjesto_rada   = (splitDescription (i.Element(xn "description").Value)).[1],
                            Rok_za_prijavu = (splitDescription (i.Element(xn "description").Value)).[2],
                            Id_category = fst items )
                        )
                     try
                        Connections.db.DataContext.SubmitChanges()
                        printfn "Successfully inserted new rows."
                     with
                       | exn -> printfn "Exception:\n%s" exn.Message
                       
