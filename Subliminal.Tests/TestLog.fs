module Subliminal.Tests.TestLog

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote

[<Property>]
let ``emits entries`` (entry1: obj) (entry2: obj) =
    let log = Log<obj>()
    let observer = TestObserver()
    use subscription = log.Subscribe(observer)
    log.Append(entry1)
    log.Append(entry2)
    test <@ observer.ObservedValues = [ entry1; entry2 ] @>
    test <@ not observer.ObservableCompleted @>
