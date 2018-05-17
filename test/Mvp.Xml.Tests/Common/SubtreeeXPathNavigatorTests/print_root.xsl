<?xml version="1.0" encoding="UTF-8" ?>
<stylesheet version="1.0" xmlns="http://www.w3.org/1999/XSL/Transform" >
  <output method="text" /> 
  <template match="/">
     Root node is  <value-of select="local-name(//*[position() = 1])" /> 
  </template>
</stylesheet>