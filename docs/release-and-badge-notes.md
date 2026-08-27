# Release & Badge Maintenance Notes

Notes on two problems that made the README badges show wrong information, and how they were fixed.

## 1. The release badge showed `v1.0.0` instead of `v1.3.2`

### Symptom

The README release badge read `RELEASE v1.0.0`, even though `v1.3.2` existed and was the newest version.

### Cause

GitHub does **not** pick the "latest" release by version number. It picks the most recently *published* release. The release records looked like this:

```
v1.3.2  id=367222809  created=2026-08-08T14:34:44Z  published=2026-08-08T14:37:11Z
v1.3.1  id=367221621  created=2026-08-08T14:29:48Z  published=2026-08-08T14:31:58Z
v1.3.0  id=367218866  created=2026-08-08T14:19:11Z  published=2026-08-08T14:19:48Z
v1.2.0  id=367214980  created=2026-08-08T14:00:56Z  published=2026-08-08T14:03:14Z
v1.1.0  id=367170988  created=2026-08-08T10:02:54Z  published=2026-08-08T10:06:16Z
v1.0.0  id=367833008  created=2026-08-08T09:36:09Z  published=2026-08-10T10:29:18Z
```

Two fields are easy to confuse:

- `created_at` comes from the date of the commit the tag points at. For `v1.0.0` that is 2026-08-08 09:36, which is genuinely the oldest commit.
- `published_at` is when the Release page itself was created on GitHub. For `v1.0.0` that is 2026-08-10 — two days *after* `v1.3.2`.

The release IDs confirm the order in which the records were created: `v1.0.0` has id `367833008`, higher than every other release including `v1.3.2` (`367222809`). IDs are handed out in increasing order, so the `v1.0.0` Release page was added last — a Release page was created after the fact for an older, already-existing tag.

Because `GET /releases/latest` sorts by `published_at`, it returned `v1.0.0`, and shields.io faithfully rendered that.

### Fix

Mark the correct release as latest explicitly:

```bash
gh api -X PATCH repos/muhammetozeski/StrongPasswordGenerator/releases/367222809 -f make_latest=true
```

Verify:

```bash
gh api repos/muhammetozeski/StrongPasswordGenerator/releases/latest --jq '.tag_name'
# v1.3.2
```

### Avoiding it next time

When creating a Release page for a tag that is older than the current newest release, pass `--latest=false` so it does not steal the "latest" marker:

```bash
gh release create v0.9.0 --latest=false --notes "..."
```

## 2. The license badge showed `REPO NOT FOUND`

### Symptom

After the repository was switched from private to public, the license badge still rendered `LICENSE: REPO NOT FOUND`, while `img.shields.io` itself already returned `LICENSE: MIT`.

### Cause

GitHub does not embed README images directly. It proxies them through `camo.githubusercontent.com`, which caches the fetched image. The badge had been fetched while the repository was still private, so shields.io answered "repo not found", and camo stored that response.

Camo's cache key is derived from the source URL — the hex path segment in the camo URL is the hex-encoded source URL. As long as the URL stays byte-identical, camo keeps serving the stored image. Sending `PURGE` to the camo URL returned HTTP 200 but did not change the served content.

### Fix

Change the source URL so it hashes to a different camo entry, forcing a fresh fetch. Reordering the query parameters is enough and leaves the rendered badge identical:

```diff
-https://img.shields.io/github/license/muhammetozeski/StrongPasswordGenerator?style=for-the-badge&color=blue
+https://img.shields.io/github/license/muhammetozeski/StrongPasswordGenerator?color=blue&style=for-the-badge
```

Verify by reading what camo actually serves, not what shields.io serves:

```bash
curl -s "https://github.com/muhammetozeski/StrongPasswordGenerator" | grep -o 'https://camo[^"]*'
curl -s "<camo-url>" | grep -o 'aria-label="[^"]*"'
```

### Avoiding it next time

Never verify a README badge by requesting `img.shields.io` directly — that is not the URL the page renders. Always read the `camo.githubusercontent.com` URL taken from the rendered page.
