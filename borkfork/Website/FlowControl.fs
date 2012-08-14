
namespace pullData
open System.Collections
open System.Xml
open System.Linq
open System.Xml.Linq
open borkData

open Parsing

open Commons
module FlowControl =

    let chooseParser()   = 
        let items = Connections.pullRawData()
        let h = Seq.length items;
        for i in items do
            match fst i with
                | 2L ->  ParseMojPosaoNet.main (snd i)
                     // ohr.parsePosaohr i |> Parsing.ParsePosaohr.
                | 1L -> ParsePosaohr.main i |> Parsing.ParsePosaohr.returnToken
                | _  -> failwith("tralalal")

    let updateDb() = 
        let timesNumber = Timer.getTimes.Length
        match timesNumber with
            | 0  -> Timer.updateTime() // doesn't work properly EDIT LATER
            | 1  -> Timer.updateTime()
            | _  -> if Timer.calculateTime((snd Timer.getTimes.[timesNumber-1]), (snd Timer.getTimes.[timesNumber-2])) >= 1 then
                        (* 
                            before doing anything we must insert raw data into database
                            pushData calls getRawData which https sites from category table
                            after this we chooseParser
                        *)
                        Connections.pushRawData()
                        chooseParser()
                           
                        Timer.updateTime()
                    
            
