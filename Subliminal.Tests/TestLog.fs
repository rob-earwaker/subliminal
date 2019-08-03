module Subliminal.Tests.TestLog

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote

[<Property>]
let ``entry is observed`` (entry: obj) =
    let log = Log<obj>()
    let observer = TestObserver()
    use subscription = log.Subscribe(observer)
    log.Append(entry)
    test <@ not observer.ObservableCompleted @>
    test <@ observer.ObservedValues = [ entry ] @>
