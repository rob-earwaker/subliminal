namespace Subliminal.Tests

open Subliminal
open Swensen.Unquote
open System.Collections.Generic
open Xunit

module ``Test Log<'Data>`` =
    [<Fact>]
    let ``emits data when logged`` () =
        let log = Log<obj>()
        let subscriber = Queue()
        log |> Log.subscribeForever subscriber.Enqueue
        let data1 = obj()
        let data2 = obj()
        let data3 = obj()
        log.Log(data1)
        log.Log(data2)
        log.Log(data3)
        test <@ subscriber.ToArray() = [| data1; data2; data3 |] @>

    [<Fact>]
    let ``subscribers receive the same data`` () =
        let log = Log<obj>()
        let subscriber1 = Queue()
        let subscriber2 = Queue()
        log |> Log.subscribeForever subscriber1.Enqueue
        log |> Log.subscribeForever subscriber2.Enqueue
        let data1 = obj()
        let data2 = obj()
        let data3 = obj()
        log.Log(data1)
        log.Log(data2)
        log.Log(data3)
        test <@ subscriber1.ToArray() = [| data1; data2; data3 |] @>
        test <@ subscriber2.ToArray() = [| data1; data2; data3 |] @>
