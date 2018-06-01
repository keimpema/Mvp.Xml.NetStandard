<?xml version='1.0'?>
<!--
<<<<<<< xinclude.xsl
    Original version copyright 2002 by W3C.
    All Rights Reserved.
=======
    XSL Stylesheet for documenting XML Inclusions(XInclude) conformance tests.
    Tested against Xalan processor.
>>>>>>> 1.0

    XSL 2002-03-07 Stylesheet for documenting XInclude conformance tests.

    This expects to be run on a document matching the DTD that was
    defined for merging collections of self-descriptive XInclude tests.

    Since all those collections will have (by design) the same test
    architecture, this includes boilerplate describing that design,
    to be used by all test documentation.

    
-->


<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="html" indent="yes"/>

    <!-- ROOT:  write an HTML wrapper -->
	<xsl:variable name="root-path">http://xw2k.sdct.itl.nist.gov/carmelo/XInclude/</xsl:variable>
    <xsl:template match="/testsuite">
	
	<!-- XHTML namespace -->
	<html><head>
	    <title>XInclude Conformance Tests</title>
	    <meta http-equiv="Content-Type"
		content="text/html;charset=utf-8"/>
	    <style>
		<xsl:comment>
		    BODY {
			font-family:	Lucida, Helvetica, Univers, sans-serif;
		    }
		     H2, H3, H4 {
			text-align:	left;
			color:		#0066CC;
		    }
		</xsl:comment>
	    </style>
	    <style>
		<xsl:comment>
		    a:hover { 
			color:		white;
			background-color: blue;
		    }
		</xsl:comment>
	    </style>
	</head><body bgcolor='#ffffff'>
            <center><h1><font color="#0066cc">XInclude Test Suite</font></h1></center>



	    <p/>
	    <center>
	    <table border="4" width="90%">
	    <tr >
	    <td>
	    <table>
	    <tr >
	    <td width="35%"><img src="http://www.w3.org/Icons/w3c_home" width="200" height="100"/></td>
	    <td width="30%"/>
	    <td width="35%" align="right"><img src="http://www.nist.gov/itl/div897/images/nist_home.gif" width="350" height="125"/></td>
            </tr>
	    </table>
	    <center>
	    <table>
	    <tr>
	    <td ><b><font size="+2">A Joint Development Effort</font></b></td>
	    </tr>
	    </table>
            </center>
	    </td>
	    </tr>
	    </table>
	    </center>
	    <p/>

	    <h4>W3C XML Core Working Group<br/>XInclude 1.0 Test Suite<br/>13 February 2003</h4>
            <dl>
	    <dt><b>This version:</b></dt>
	    <dd><ul>
	       <li><a href="http://xw2k.sdct.itl.nist.gov/carmelo/xinclude/xinclude.html">
		   http://xw2k.sdct.itl.nist.gov/carmelo/xinclude/xinclude.html</a>		 
	       </li>
	    </ul>
	    </dd>
	    <p/>
	    <dt><b>Current version:</b></dt>
	    <dd><ul>
	       <li><a href="http://xw2k.sdct.itl.nist.gov/carmelo/xinclude/xinclude.html">
		   http://xw2k.sdct.itl.nist.gov/carmelo/xinclude/xinclude.html</a>
	       </li>
	    </ul>
	    </dd>
	    <p/>
	    <dt><b>Test Archive:</b></dt>
	    <dd><ul>
	    <li><a href="http://xw2k.sdct.itl.nist.gov/carmelo/xinclude/xinclude0322.zip">http://xw2k.sdct.itl.nist.gov/carmelo/xinclude/xinclude0322.zip</a>
	    </li>
	    </ul></dd>
	    <p/>
	    <dt><b>W3C XML Core Working Group:</b></dt> 	
	    <dd><ul><li><a href="http://www.w3.org/">http://www.w3.org/</a>
	    </li></ul></dd>
	    <p/>
	    <dt><b>XML Inclusions(XInclude) Version 1.0:</b></dt> 	
	    <dd><ul><li><a href="http://www.w3.org/TR/xinclude">http://www.w3.org/TR/xinclude</a>
	    </li></ul></dd>
	    <p/>
	    <dt><b>Comments:</b></dt> 	
	    <dd><ul><li>Sandra Martinez, NIST <a href="mailto:sandra.martinez@nist.gov">&lt;sandra.martinez@nist.gov&gt;</a>
	    </li></ul></dd>
	    <p/>
	    </dl>
	    <a name="contents"/>
            <h2>Table of Contents</h2>
	    <ol >
		<li><a href="#intro">Introduction</a></li>
		<li><a href="#categories">Test Case Descriptions</a></li>
                <li><a href="#contrib">Contributors</a></li>
	    </ol>
	    <a name="intro"/>
	    <h2>1. Introduction </h2>
	    <p> XInclude is a generic mechanism that is used to merge XML documents. XInclude uses
                elements, attributes and URI references which are existing XML constructs.</p>
	    <p>
	    Conformance tests can be used by developers, content creators, and 
            users alike to increase their level of confidence in product quality. In
	    circumstances where interoperability is necessary, these tests can also
	    be used to determine that differing implementations support the same set
            of features. </p>
           
            <p>This report provides supporting documentation for all of the tests
            contributed by members of the <i>W3C</i> XML Core Working Group. 
	    Sources from which these tests have been collected
	    include: <em>
		<xsl:for-each select="testcases">
		    <xsl:value-of select="@creator"/>
		    <xsl:text>; </xsl:text>
		</xsl:for-each>
	    </em>.  It is anticipated that this report will supplement the actual tests.</p>  

	    <p>Comments/suggestions should be 
            forwarded to <a href="mailto:www-xml-xinclude-comments@w3.org">public XInclude comments mailing list</a>.</p> 

	    <a name="categories"/>
	    <h2>2.  Test Case Description</h2>

	    <p> This section of this report contains descriptions of test cases, 
		each test case includes a binary test. The test suite consists of a 
		series of simple binary conformance tests, whereby a processor is deemed 
		either to accept  (a positive test) or reject  (a negative test) the test. 
		An expected result document is provided for the positive tests to compare 
		the output of the processor with the reference file. 
		Expected results are as accurate as possible, it is conceivable that a 
		few files may diverge somewhat from the testcase description, however 
		the idea is still captured. Negative tests are intended to 
		identify expected errors as defined by the specification. Negative test are not 
		accompanied by expected results.</p>
	      <p>For an implementation to claim conformance it must pass all positive and negative 
		tescases, which means that the implementation performed exactly as expected. </p>

		<p>A description for each test is presented in the following table as well as a 
		section identifier from the Xinclude recommendation and the Colletion from 
		which the test was originated. The test description is intended to have enough 
		detail to evaluate diagnostic messages. Given the Test ID and the Collection, 
		each test can be uniquely identified.</p>

		<xsl:apply-templates select="testcases"/>
                   


	    <a name="contrib"/>
	    <h3>4.  Contributors (Non-normative)</h3>

	    <p> A team of volunteer members have participated in the
	    development of this work.  Contributions have come from:
	    </p>
	    <ul>
                <li>Sandra I. Martinez, NIST</li>
		<li>Daniel Veillard, Red Hat Network</li>
                <li>John Evdemon, FourThought</li>
            </ul>

	</body></html>
    </xsl:template>


    <xsl:template match="testcases">
       
       <xsl:variable name="num-of-tests">
           <xsl:value-of select="count(testcase)"/>
       </xsl:variable>

       <xsl:choose>
         <xsl:when test="$num-of-tests=0">
	   <table>
	     <tr>
		<td bgcolor="#ffffcc">Currently there ARE NOT any tests for this test suite</td>
	    </tr>
	   </table>
 
         </xsl:when>

    <xsl:otherwise>          
         <xsl:for-each select="testcase">
		
                <table width="100%">
                    <tr valign="top">
	                <td width='40%'><table bgcolor='#eeeeff' border='1' width='100%'>
		    <tr>
		    <td width='50%'><b>Sections [Rules]:</b></td>
		    <td bgcolor='#ffffcc'>
			    <xsl:value-of select="section"/></td>
		    </tr>
		    <tr valign="top">
		        <td width='50%'><b>Test ID:</b></td>
		         <td bgcolor='#ffffcc'>
			 <xsl:variable name="test-path"><xsl:value-of select="concat(../@creator,'/',@href)"/></xsl:variable>
			    	<a href="{$root-path}{$test-path}"><xsl:value-of select="@id"/></a>		
			</td>   
		    </tr>
                    <tr valign="top">
			    <td width='50%'><b>Collection:</b></td>
			    <td bgcolor='#ffffcc'><xsl:value-of select="../@creator"/></td>
                        </tr>
	        </table>
                </td>
	        <td bgcolor='#ccffff'>
		   <p><xsl:value-of select="description"/></p>
		   <xsl:if test="output">
		      <p>There is an output test associated with this input file.</p>
		   </xsl:if>
	        </td>
	    </tr>
	    </table>
            </xsl:for-each>
         </xsl:otherwise>         
         </xsl:choose>      
</xsl:template>


    <xsl:template match="EM">
	<em><xsl:apply-templates/></em>
    </xsl:template>
    <xsl:template match="B">
	<b><xsl:apply-templates/></b>
    </xsl:template>
</xsl:stylesheet>
