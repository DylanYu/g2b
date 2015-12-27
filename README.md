# g2b
This tool can sync your AdWords account to your BingAds account.
It uses AdWords SDK and BingAds SDK.
You need to prepare refresh token and dev token, etc. by yourself.

## How to use
1. Use BingAds Web UI Google Import
2. g2b.exe init - this command will create id mapping on your local machine
3. g2b.exe touch - create a .LastSync file
4. g2b.exe sync - sync anything we support


## What has been supported
Keyword: CpcBid, Status

