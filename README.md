<h2>"# XPathOptimizer" </h2>

<hr/>
<p>[Parameters]</p>

<p> <strong> -x, --xml=VALUE        </strong>  </p><p>   The path to the XML file                                                                                                      </p>
<p> <strong> -p, --xpath=VALUE      </strong>  </p><p>   The XPath to the desired element (if only one path needs to be optimized)                                                     </p>
<p> <strong> -f, --xpathsfile=VALUE </strong>  </p><p>   The path to a file containing multiple XPaths to be optimized (1 per line)                                                    </p>
<p> <strong> -c, --count=VALUE      </strong>  </p><p>   The count of the sudgestions to be given for each XPath (default is 10)                                                       </p>
<p> <strong> -a, --attr=VALUE       </strong>  </p><p>   List of attributes that are considered important and stable in your xml format, seperated by commas. (default: 'id,name')     </p>
<p> <strong> -s, --speed            </strong>  </p><p>   If 'True', the program will attempt to prioratize faster paths, even if other paths are more readable                         </p>
<p> <strong> -h, --help             </strong>  </p><p>   Shows the help                                                                                                                </p>

<hr/>
<p>[Example Usage]</p>

<p>XPathOptimizer.Main.exe -x="XmlFile.xml" -c=10 -p="//body[1]//div[6]/div[4]/div[5]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]"</p>

<hr/>
<p>[Result Output:]</p>

<p>Failed to parse XML. Attempting to treat it as HTML</p>
<p>Results for : //body[1]//div[6]/div[4]/div[5]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]</p>

<p>7.153  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']//div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']//div[@id='hdtb-msb-vis']/div[2]          </p>
<p>7.153  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']/div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']/div[@id='hdtb-msb-vis']/div[2]            </p>
<p>7.139  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']//div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']/div/div[@id='hdtb-msb-vis']/div[2]       </p>
<p>7.139  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']/div/div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']//div[@id='hdtb-msb-vis']/div[2]       </p>
<p>7.139  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']/div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']/div/div[@id='hdtb-msb-vis']/div[2]        </p>
<p>7.139  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']/div/div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']/div[@id='hdtb-msb-vis']/div[2]        </p>
<p>7.128  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']/div/div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']/div/div[@id='hdtb-msb-vis']/div[2]    </p>
<p>7.053  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']//div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div[@id='hdtb-msb']//div/div[2]                              </p>
<p>7.053  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']//div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']//div/div[@id='hdtb-msb-vis']/div[2]                          </p>
<p>7.053  =>  /body[@id='gsr']/div[@id='main']/div[@id='cnt']/div[@id='top_nav']//div[@id='hdtb']/div[@id='hdtbSum']/div[@id='hdtb-s']/div//div[@id='hdtb-msb-vis']/div[2]                          </p>
