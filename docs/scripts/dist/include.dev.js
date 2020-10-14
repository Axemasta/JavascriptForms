"use strict";

// To include a HTML file use this code
// <script> include('path/to/file.html', document.currentScript) </script>
function include(file, element) {
  req(file).then(function (code) {
    return insertCode(code, element);
  })["catch"](function () {
    return showError();
  });
}

function insertCode(code, referenceElem) {
  var _getTags = getTags(code, 'script'),
      scripts = _getTags.tags,
      htmlAndCSS = _getTags.codeWithoutTags;

  var _getTags2 = getTags(htmlAndCSS, 'style'),
      styles = _getTags2.tags,
      html = _getTags2.codeWithoutTags;

  referenceElem.insertAdjacentHTML('beforebegin', html); // HTML

  styles.forEach(function (style) {
    return insertTag(style, 'style');
  }); // Styles

  scripts.forEach(function (script) {
    return insertTag(script, 'script');
  }); // Scripts
}

function showError() {
  console.log('Error getting file');
}

function insertTag(code, tagName) {
  var referenceElem = document.querySelector("[data-".concat(tagName, "-tag]"));
  var elem = document.createElement(tagName);
  elem.textContent = code;
  referenceElem.parentNode.insertBefore(elem, referenceElem.previousSibling);
  referenceElem.remove();
}

function getTags(code, tagName) {
  var patternForGettingTag = new RegExp("(?<=<".concat(tagName, ">)(.*?)(?=</").concat(tagName, ">)"), 'gs');
  var patternForRemovingTag = new RegExp("<".concat(tagName, ">(.*?)/").concat(tagName, ">"), 'gs');
  var tags = code.match(patternForGettingTag);
  var codeWithoutTags = code.replace(patternForRemovingTag, "<".concat(tagName, " data-").concat(tagName, "-tag></").concat(tagName, ">"));
  return {
    'tags': tags === null ? [] : tags,
    'codeWithoutTags': codeWithoutTags
  };
} // Requesting file


function req(fileName) {
  return new Promise(function (res, rej) {
    var req = new XMLHttpRequest();

    req.onreadystatechange = function () {
      if (this.readyState == 4) {
        if (this.status != 200) return rej();
        res(this.responseText);
      }
    };

    req.open("GET", fileName, true);
    req.send();
  });
}