# Monetisation Principles

## Goal

Monetisation should support the game without damaging trust, retention or game balance.

Do not add live ads, purchases or subscriptions before the core loop is worth returning to.

## Preferred Model

Start with:

- optional rewarded ads;
- remove-forced-ads purchase;
- cosmetic packs;
- campaign or biome content packs;
- starter pack, if the economy supports it honestly.

Consider a monthly pass only after retention and content cadence justify it.

## Avoid

- banner ads during gameplay;
- surprise rewarded-ad outcomes without clear disclosure;
- making the game deliberately tedious to sell currency;
- paywalls that block basic fun;
- subscriber-only raw power that breaks balance;
- expiry on purchased currency.

## Service Boundary

Ads and purchases must sit behind services so gameplay logic does not depend directly on an SDK.

Expected interfaces later:

- `IAdsService`;
- `IPurchaseService`;
- `IEntitlementService`;
- `IRemoteConfigService`;
- `IAnalyticsService`.

The first implementation can be fake or local, but the boundary should exist early.

Current repo status:

- analytics event names and required properties live in a tested catalogue;
- a local analytics recorder validates event names and required properties;
- rewarded-ad opportunities live in a tested catalogue;
- a local rewarded-ad offer tracker enforces per-run offer limits;
- debug rewarded-ad prompts can be claimed or skipped without loading a real ad;
- no live ad, purchase, subscription or backend SDK has been added.

## Rewarded Ads

Rewarded ads should be explicit and optional.

Good examples:

- watch ad for extra post-level scrap;
- watch ad to reroll a daily challenge reward;
- watch ad for one extra failed-level retry.

Bad examples:

- interrupting active gameplay;
- hiding the reward amount;
- making ad watching the default way to progress.
