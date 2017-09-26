module RxCalc.Infrastructure

open System

[<CLIMutable>]
type Envelope<'T> = {
    id: Guid
    content: 'T }

[<CLIMutable>]
type Created = {
    id: string
    url: string }

let envelope id data = {
    id = id
    content = data }

let envelopeWithDefaults data = 
    let guid = Guid.NewGuid()
    envelope guid data
