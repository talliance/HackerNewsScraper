# HackerNewsScraper
Return a json output of the top news posts from HackerNews

Because the HackerNews top news posts are in a table with 2 rows containing the info I need and a row to seperate posts, I decide that it would be more efficient to download the enitire page as a HTML document and then use xpath to loop through and query the document for the info I needed.
This meant that the only libraries I ended up using were:
1) HtmlAgilityPack - to get the web page as a document from a uri
2) Newtonsoft.Json - to serialize the the list I create into JSON

How to Use:
1) Open either a powershell or cmd window
2) Navigate to the folder with the exe in it, using the command:
    cd [your path]\HackerNewsScraper\HackerNewsScraper\bin\Release
3) Then run the command:
    HackerNewsScraper.exe -- posts n
    (n = the number of posts you want to return)
4) That's it! It will now return the number of specified top news posts
