module Subliminal.Tests.TestLog

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote

[<Property>]
let ``entry is observed`` entry =
    let log = Log<obj>()
    let observer = TestObserver()
    use subscription = log.Subscribe(observer)
    log.Append(entry)
    test <@ not observer.ObservableCompleted @>
    test <@ observer.ObservedValues.Count = 1 @>
    test <@ observer.ObservedValues.[0] = entry @>
