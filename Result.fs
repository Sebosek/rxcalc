module RxCalc.Result

open System
open System.Collections.Generic

open Microsoft.AspNetCore.Http

open Giraffe.HttpContextExtensions
open Giraffe.HttpHandlers

open RxCalc.Infrastructure
open RxCalc.Model


let add (list : List<Envelope<SumResult>>) result =
    result |> list.Add

type IResult =
    abstract member About : Guid -> Envelope<SumResult>

type Result (observable : IObservable<Envelope<SumResult>>) =
    let results = List<Envelope<SumResult>>([])
    let mutable subscription : IDisposable = null
    do
        subscription <- observable.Subscribe (fun sum -> 
            add results sum)
    interface IResult with
        member this.About (id : Guid) =
            results |> Seq.find (fun x -> x.id = id)
    interface IDisposable with
        member this.Dispose () =
            subscription.Dispose()

// let result (service : IResult) id =
//     id |> service.About |> json
// let fail () =
//     text "Unable to parse ID"
// let parseId id success fail =
//     match Guid.TryParse(id) with
//         | true, guid -> success guid
//         | false, _ -> fail

let controller id =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let service = ctx.GetService<IResult>()
        //parseId id (service |> result) (fail)

        let result = service.About (id |> Guid.Parse)
        json result next ctx