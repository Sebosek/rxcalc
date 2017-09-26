module RxCalc.Extensions

open System

open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Builder

open RxCalc.Infrastructure
open RxCalc.Result
open RxCalc.Model
open RxCalc.Agent
open RxCalc.Sum

type IServiceCollection with
    member this.AddRxCalc () =
        let processor = new SumProcessor()

        this.AddSingleton<ISumProcessor>(fun x -> processor :> ISumProcessor) |> ignore
        this.AddSingleton<IObservable<Envelope<Sum>>>(fun x -> 
            processor :> IObservable<Envelope<Sum>>) |> ignore
        this.AddSingleton<IResult, Result>() |> ignore
        this.AddSingleton<Agent>() |> ignore
        this.AddSingleton<IObservable<Envelope<SumResult>>>(fun x -> 
            x.GetService<Agent>() :> IObservable<Envelope<SumResult>>) |> ignore


type IApplicationBuilder with
    member this.UseRxCalc () =
        this.ApplicationServices.GetService<IResult>() |> ignore
        this.ApplicationServices.GetService<Agent>() |> ignore