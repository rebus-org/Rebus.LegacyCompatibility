# Rebus.LegacyCompatibility

[![install from nuget](https://img.shields.io/nuget/v/Rebus.LegacyCompatibility.svg?style=flat-square)](https://www.nuget.org/packages/Rebus.LegacyCompatibility)

Provides (EXPERIMENTAL!) legacy compatibility for [Rebus](https://github.com/rebus-org/Rebus) for

![](https://raw.githubusercontent.com/rebus-org/Rebus/master/artwork/little_rebusbus2_copy-200x200.png)

---

"Rebus 1" (i.e. Rebus versions <= 0.84.0) had slightly different headers and some less than perfect defaults regarding message serialization.

This makes Rebus 1 and Rebus 2 (Rebus versions >= 0.90.0) non-compatible.

Rebus 2 is so extensible though that it can be brought to be pretty close to wire-compatible with Rebus 1 by taking advantage of its message pipelines (and some more tricks).

By including this package, you can

	Configure.With(...)
		.(...)
		.Options(o => o.EnableLegacyCompatibility())
		.Start();

in order to make your endpoint compatible with Rebus 1.

PLEASE NOTE that ALL of your Rebus 2 endpoints need this configuration if you choose to use it! The recommended migration approach is as follows:

1. Update your endpoints to Rebus 2 - one at a time - with legacy compatibility turned on(*).
1. When all endpoints are Rebus 2: Figure out how to empty all of your queues - most likely by stopping your endpoints in the right order.
1. When all queues are empty: Remove the legacy compatibility option from all endpoints.
1. Start the world again.

----

(*) The legacy compatibility feature does NOT help you migrate sagas, subscriptions and timeouts. You need to handle these on your own, manually, for each endpoint you upgrade.