# TESPA-HS-Meta

TESPA-HS-META performs some automated analytics for a Hearthstone tournament.\
It reads a Google Sheets doc for team names, colleges, and deck codes.
[you can read about deck codes here](https://hearthsim.info/docs/deckstrings/)

## Pre-Requisites (dev only)
This is a Microsoft Visual Studio Project and will work best with Visual Studio on Windows. If you know what you're doing, feel free to edit using something else.

## Installation

**NOTE:** These instructions are for setting up a dev environment. If you only want to use the program as-is, clone this repo and then skip to step 2

#### Step 1: cloning this & the HearthDB library
Clone this repo and [the HearthDB C# library on GitHub (courtesy of HearthSim)](https://github.com/HearthSim/HearthDb).\
Put them in the same parent directory. Your directory should look like this: \
/{parent}/**HearthDB**/HearthDB.sln\
/{parent}/{this repo}/TESPA HS META.sln


#### Step 2: Running the app for the first time
Run "TESPA HS META.exe" in /TESPA HS META/bin/publish/ \
You should get an OAuth prompt from Google. Accept it. I only read from [this spreadsheet](https://docs.google.com/spreadsheets/d/e/2PACX-1vSFlA6LIYylbC7t2l9u1FWMfFU950V-henjF-jgEyD75lV4kkSBcymriRgW4_PtrkhmCSmnKcEX-KxU/pubhtml) which is a manual copy of [TESPA's public spreadsheet](https://docs.google.com/spreadsheets/d/e/2PACX-1vRXjPL95ONrvuvlfH1nfDeMMr6UkfxeoOCGcJ-w_AaSgV8-pQpzw90dZLGMYLV03wf8hGQqoU8St7WJ/pubhtml)




## License
[MIT](https://choosealicense.com/licenses/mit/)

## Acknowledgements
* Thank you to my two teammates, my roommate, and my friends for encouragement & support
