In-Close4 CONCEPT MINING AND TREE BUILDING PROGRAM - 64 bit version

Copyright (c) 2017 Simon Andrews s.andrews@shu.ac.uk

In-Close4 mines .cxt (Formal Context) and FIMI .dat files for Formal Concepts (http://www.upriss.org.uk/fca/fca.html)
In-Close4 builds and outputs concept trees in JSON format for the D3.js Collapsible Tree Format
To build and view a tree, output the tree using In-Close4 with output option 4. Open the tree visualiser in Chrome web browser (or other browser, but not IE) at: http://homepages.shu.ac.uk/~aceslh/fca/fcaTree.html
Then choose and upload the concepts.json file.

There should be eleven other files with this README:

In-Close4.cpp		//main source code
hr_time.cpp		//source code for timers***
hr_time.h		//header file for timers***
InClose4-MS2012.exe	//executable
InClose4-MS2010.exe	//executable
inclose.ico		//icon
liveinwater.cxt		//sample context file*
lattice.cxt		//sample context file*
tealady.cxt		//sample context file*
mushroomep.cxt		//sample context file**
LICENSE.txt		//MIT open-source license

NOTE: Running the executable requires the Microsoft Redistributable 2010 (for In-Close4-MS2010.exe) and Microsoft Redistributable 2012 (for In-Close4-MS2012) to be installed on your machine (which it probably will be if you have various Microsoft products). If not the MS2010 Redistributable can be installed from here: http://www.microsoft.com/en-gb/download/details.aspx?id=14632

*NEW FEATURES*
In-Close4 outputs a concept tree in JSON format that can be visualised at: http://homepages.shu.ac.uk/~aceslh/fca/fcaTree.html
In-Close4 makes use of 64 bit architecture to be faster (64-bitwise operation) and to cope with larger contexts/number of concepts  
(more memory available).
In-Close4 can now input FIMI .dat files.
Concepts are now output in JSON format or csv.
BUG-FIX: issue with slow pre-processing (sorting) when file contains many repeated objects (see Kirchberg et al 2014: Formal Concept  
Discovery in Semantic Web Data) now fixed by using a better version of Quick Sort.

There are also the following options:
-Output the cxt file after sorting (sorts columns in order of density and sorts rows as if they were binary numbers). Note this option also means that FIMI .dat files can be converted into .cxt format.

-Specify minimum size of intent (min no of attributes in a concept).
-Specify minimum size of extent (min no of objects in a concept).

-Output concepts to a file called concepts.json in JSON format. Three options:
	1) concepts as lists of index numbers of objects and attributes.
	2) concepts as lists of names of objects and attributes.
	3) concepts with arrays for many-valued attributes, e.g. if the objects share the attributes "location-Sheffield" and
		"location-London" the corresponding JSON construction will be "location" : ["Sheffield","London"].
	4) output a Concept Tree (concepts.JSON) where child concepts are witjhin parent cocnepts. Can be visualised at http://homepages.shu.ac.uk/~aceslh/fca/fcaTree.html
	5) output a Concept Tree (concepts.JSON) where a concept includes the index number of its parent.
	6) output concepts as a csv file.

In-Close4 outputs the count of concepts (that satisfy the minimum support specified).
In-Close4 outputs the time taken for pre-processing (sorting) and the time taken for concept mining.
In-Close4 outputs the breakdown of concept count by size in two files:
-noConsByBsize.txt: List of <number of attributes> - <number of concepts having that number of attributes>
-noConsByAsize.txt: List of <number of objects> - <number of concepts having that number of objects>

In-Close4 outputs the reduced context (cxt) file that results from the minimum sizes of intent and extent specified by the user, also removing objects (rows) and attributes (columns) that are not involved in concepts that satisfy the minimum support.
Thus it is possible to produce a context that only contains the 'largest' concepts, making it possible to visualise the resulting lattice using tools such as 'Concept Explorer' (http://sourceforge.net/projects/conexp/).


Max Values used in executable
#define MAX_CONS 23000000	//max number of concepts
#define MAX_COLS 5000		//max number of attributes
#define MAX_ROWS 200000		//max number of objects
#define MAX_FOR_B 40000000	//memory for storing intents
#define MAX_FOR_A 300000000	//memory for storing extents

Change these in the source code as required and available RAM allows. Current values are for standard 64 bit Windows PC with 8GB RAM.

In-Close4 is released under the Free and Open Source Software, MIT License. However, the author politely requests that users of the Software please inform him (by email: s.andrews@shu.ac.uk) what use they have made or are making of the Software. This is purely to help the author record what impact the Software has had, whether in private use, commercial application, internal/non-commercial application, research or public benefit. Thank you.


* liveinwater.cxt, lattice.cxt and tealady.cxt are from http://www.upriss.org.uk/fca/examples.html, which also contains references to the original sources of these contexts.

** mushroomep.cxt is a context derived from the Mushroom data set at the UC Irvine Machine Learning Repository (http://archive.ics.uci.edu/ml/).

***hr_time.cpp and hr_time.h use the standard C++ CPU query function QueryPerformanceCounter to accurately time code execution.
