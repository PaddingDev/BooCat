# Scripts of BooCat

This file contains the most useful scripts to help O & M BooCat services.

## Instructions

### For Azure Function Deployer

After deploying the function app, you can just use function as it, no action required.
However, if you need a rate limit, except the Azure API Management or WAFs, you can deploy
front limit services to implement such feature. I use Nginx as a proxy service before the
function to prevent abuse. So just use following nginx and similar things:

```bash
sudo bash .\init-ngx.sh
```

### For Web-API Deployer

Before everything, you should install basic environment incl. `git`, `dotnet`, `wget` and
`ca-certificates`. With following script to install with one-key:

```
sudo bash .\install-env.sh
```

After installing all essential tools, install the api as a service is recommanded:

```bash
sudo bash .\install-api.sh
```

Then your API should be alright to run.

If you want to upgrade the API to the newest version, use following command. It supports auto
fall back which allows upgrading services soothly.

```bash

```bash
sudo bash .\upd-api.sh
```
