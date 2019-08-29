namespace Subliminal.Tests

open FsCheck.Xunit
open Subliminal
open Swensen.Unquote
open System

module ``Test Log<TEntry>`` =
    [<Property>]
    let ``emits entries when entries appended`` (entry1: obj) (entry2: obj) =
        let log = Log<obj>()
        let observer = TestObserver()
        use subscription = log.Subscribe(observer)
        log.Append(entry1)
        log.Append(entry2)
        test <@ observer.ObservedValues = [ entry1; entry2 ] @>
        test <@ not observer.ObservableCompleted @>
        
    [<Property>]
    let ``observers receive the same entries`` (entry1: obj) (entry2: obj) =
        let log = Log<obj>()
        let observer1 = TestObserver()
        let observer2 = TestObserver()
        use subscription1 = log.Subscribe(observer1)
        use subscription2 = log.Subscribe(observer2)
        log.Append(entry1)
        log.Append(entry2)
        test <@ observer1.ObservedValues = [ entry1; entry2 ] @>
        test <@ observer2.ObservedValues = [ entry1; entry2 ] @>
            