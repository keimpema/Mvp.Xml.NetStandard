<stylesheet version="1.0" xmlns="http://www.w3.org/1999/XSL/Transform" >
  <output indent="yes"/>
  <template match="/">
	  <page>
	  <call-template name="WriteScriptTag">
		  <with-param name="js_url">foo.js</with-param>
		  <with-param name="js_base">http://clariusconsulting.net/</with-param>
	  </call-template>
	  </page>
  </template>
</stylesheet>
