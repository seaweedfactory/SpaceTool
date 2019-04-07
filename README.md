# SpaceTool
SpaceTool is a command line interface to SpaceTools library, a library for scraping data from MySpace to JSON.

# Using SpaceTool 

The tool takes arguments in the form of
```
-argument parameter
```
Multiple parameters can be used at the same time.

# Arguments

## -t, -tool
[pd|lpd|lc|pc] Tool to use, defaults to profile download.

**pd**		Profile downloader, downloads a single profile.<br>
**lpd**		List profile downloader, downloads a list of profiles.<br>
**lc**		Extracts location info from connections in profiles in directory.<br>
**pc**		Profile crawler, downloads a profile and then crawls its connections.<br>

## -u, -username
Profile name to download.

## -h, -hashkey
Hashkey for API calls.

## -s, -store_directory
Directory to store information to.

## -c, -capture_photos
[true|false] to capture photos.

## -n, -capture_connections
[true|false] to parse connections.

## -dbp, -delay_between_pages
ms between page requests.

## -dba, -delay_between_api_calls
ms between API calls.

## -d, -crawl_depth
Depth to crawl. Add 1 to process leaves.

## -l, -username_list_path
Path to list file for list downloader.

## -o, -location_criteria
Profile location must contain this string to be downloaded.

## -e, -allow_empty_locations
[true|false] to process profiles with no location information.

# App.config
hashkey and store_directory can also be configured in the App.config file.

The tool will check these values if not provided by arguments.

# hashkey
To get the hashkey for use with the library, check a request header from a manual visit to MySpace.

Here are the steps to do this in Firefox.

1. Open profile page.

2. Inspect root element.
3. Go to network tab.
4. Find POST call.
5. Inspect header on call.
6. Click Edit and Resend.
7. Copy value of <b>Hash</b> parameter.
8. The key should be roughly 250 characters long or longer.
