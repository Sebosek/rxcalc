module RxCalc.Model

[<CLIMutableAttribute>]
type Sum = {
    operandA: int
    operandB: int }

[<CLIMutableAttribute>]
type SumResult = {
    operandA: int
    operandB: int
    result: int }