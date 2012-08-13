namespace Parsing 

open HtmlAgilityPack
open borkData.Connections
open Microsoft.FSharp.Text
open System.Text.RegularExpressions
open Commons


// Funkcija za formatvrijednosti (ovdje ili u commons)
open HtmlAgilityPack

    (*
         #r "C:\Users\clarksdale\Desktop\Net45\HtmlAgilityPack.dll"
         #r "FSharp.PowerPack.dll";; 
         #r "FSharp.PowerPack.Compatibility.dll";;
    *)
module ParseMojPosaoNet =
        
        // figure out behavior of this type during compilation

        //*** TEST ZA OSTALE POSLOVE

        let tokens (nodes: HtmlNodeCollection) =  seq {
            for i in nodes do
                yield [
                    i.SelectSingleNode("li[1]/h3").InnerText; //title
                    i.SelectSingleNode("li[1]/h3/a").GetAttributeValue("href", ""); // link
                    i.SelectSingleNode("li[2]/strong").InnerText; // Poslodavac

                    Others.beatufyString(i.SelectSingleNode("li[2]").InnerText) //grad 
                    i.SelectSingleNode("li[@class='deadline']/span").InnerText // rok za prijavu
                ]
        }

        let otherJobs rawData =
          let unparsedHtml = new HtmlDocument()
          // pull raw 
          unparsedHtml.LoadHtml(rawData) //http "http://www.moj-posao.net/Poslovi/11/IT-telekomunikacije/")
          let nodes =  unparsedHtml.DocumentNode.SelectNodes("//section[@class='searchlist']/div/ul")
          tokens nodes

        //*** KRAJ 
        
        
        //** FEATURED JOBS ONI SARENI NA VRHU STO SE KAO REKLAMIRAJU 
        let featuredJobs rawData = 
            let unparsedHtml = new HtmlDocument() 
            unparsedHtml.LoadHtml(rawData)
            unparsedHtml.DocumentNode.SelectNodes("//*[@id='featured-jobs']/ul/li/span/a")
        
         
        let getLinks (links: HtmlNodeCollection) = seq { 
            for i in links  do
                yield i.GetAttributeValue("href", "")
           }
   
    

        // this functions NEEDS REFACTORING 


            
        let getJobsInfo (s: string) =
            let unparsedHtml = new HtmlDocument() 
            Parsing.http (s) |> unparsedHtml.LoadHtml
            let getTitle   = unparsedHtml.DocumentNode.SelectSingleNode("//html/head/title").InnerHtml
            // |> String.split pipe doesn't work with String.,Split anymore damn it 
            let SplitTitle = getTitle.Split('-')                    

            let getRokZaPrijavu = (unparsedHtml.DocumentNode.SelectSingleNode("//div[@id='item-date']").InnerText)
            let  splitRokZaPrijavu =  getRokZaPrijavu.Split(':')

            let Title = SplitTitle.[1]
            let Mjestorada = SplitTitle.[2]
            let Poslodavac = SplitTitle.[3]
            [Title; s; Poslodavac; Mjestorada; splitRokZaPrijavu.[1].Replace(" ", ""); "2"]
        
        let tokensFeaturedJobs links = seq {
             for i in links do
                 yield getJobsInfo(i)
                }

        let pushToDb (s: seq<string list>) = 
             for i in s do
                db.ONEPOST.InsertOnSubmit( new dbSchema.ServiceTypes.ONEPOST
                    (
                      Title =   i.[0],
                      Link  = i.[1],
                      Poslodavac   = i.[2],
                      Mjesto_rada   = i.[3],
                      Id_category   = 2L,
                      Rok_za_prijavu = i.[4]
                      )
                    )
                try
                    db.DataContext.SubmitChanges()
                    printfn "Successfully inserted new rows."
                with
                    | exn -> printfn "Exception:\n%s" exn.Message
         //*** KRAJ fetaured jobs             
        let main rawData =
                otherJobs rawData |> pushToDb
               // featuredJobs rawData |> getLinks |> tokensFeaturedJobs |> pushToDb
    
            