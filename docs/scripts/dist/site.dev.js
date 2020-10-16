"use strict";

/* Scripts for the docs site. Nothing here goes into the mobile app */
$(document).ready(function () {
  HighlightCode();
});

function HighlightCode() {
  //console.log('highlighting...');
  $('pre code').each(function (i, e) {
    hljs.highlightBlock(e);
  }); //console.log('everything should be highlighted! :)');
}