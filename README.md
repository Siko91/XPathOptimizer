"# XPathOptimizer" 

[Parameters]

  -x, --xml=VALUE            The path to the XML file
  -p, --xpath=VALUE          The XPath to the desired element (if only one path needs to be optimized)
  -f, --xpathsfile=VALUE     The path to a file containing multiple XPaths to be optimized (1 per line)
  -c, --count=VALUE          The count of the sudgestions to be given for each XPath (default is 10)
  -a, --attr=VALUE           List of attributes that are considered important and stable in your xml format, seperated by commas. (default: 'id,name')
  -s, --speed                If 'True', the program will attempt to prioratize faster paths, even if other paths are more readable
  -h, --help                 Shows the help

[Example Usage]

XPathOptimizer.Main.exe -x="XmlFile.xml" -c=10 -p="//body[1]//div[6]/div[4]/div[5]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]"

[Result Output:]

Failed to parse XML. Attempting to treat it as HTML
Results for : //body[1]//div[6]/div[4]/div[5]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]

7.153  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']//div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']//div[@id='hdtb-msb-vis']/div[2]
7.153  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']/div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']/div[@id='hdtb-msb-vis']/div[2]
7.139  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']//div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']/div/div[@id='hdtb-msb-vis']/div[2]
7.139  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']/div/div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']//div[@id='hdtb-msb-vis']/div[2]
7.139  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']/div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']/div/div[@id='hdtb-msb-vis']/div[2]
7.139  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']/div/div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']/div[@id='hdtb-msb-vis']/div[2]
7.128  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']/div/div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']/div/div[@id='hdtb-msb-vis']/div[2]
7.053  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']//div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']//div/div[2]
7.053  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']//div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']//div/div[@id='hdtb-msb-vis']/div[2]
7.053  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']//div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div//div[@id='hdtb-msb-vis']/div[2]

