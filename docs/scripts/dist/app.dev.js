"use strict";

$(document).ready(function () {
  var mobile = false;
  var secretWords = ['cat', 'dog', 'hello'];
  $('#invoke-name-btn').on('click', function () {
    console.log('submitting');
    invokeCSCode($('#invoke-name-entry').val(), this);
  });

  if (mobile) {
    SpyOnUser();
  } else {
    $('#intercept-keys-start-btn').on('click', function () {
      SpyOnUser(true);
    });
    $('#intercept-keys-stop-btn').on('click', function () {
      SpyOnUser(false);
    });
  }

  function SetListeningStatus(listening) {
    var control = $('.listening-status');
    var icon = control.find('i');
    var text = control.find('span');

    if (listening) {
      text.text('Listening');
      control.removeClass('status-stopped');
      control.addClass('status-listening');
      icon.removeClass('fa-stop');
      icon.addClass('fa-spinner fa-pulse');
    } else {
      text.text('Stopped');
      control.addClass('status-stopped');
      control.removeClass('status-listening');
      icon.addClass('fa-stop');
      icon.removeClass('fa-spinner fa-pulse');
    }
  }

  function SpyOnUser(start) {
    if (!start) {
      console.log('ending spy session');
      document.onkeypress = null;
      userInputs = [];
      SetListeningStatus(false);
      return;
    }

    SetListeningStatus(true);
    console.log('now spying on user');
    var userInputs = [];
    $('#historyTextArea').text('');
    $('#intercept-keys-clear-btn').on('click', function () {
      $('#historyTextArea').text('');
      userInputs = [];
    }); //Output key press

    document.onkeypress = function (e) {
      e = e || window.event;
      userInputs.push(e.key);
      var joined = userInputs.join('');

      if (secretWords.includes(joined.toLowerCase())) {
        try {
          invokeCSCode(joined, $(':focus'));
          userInputs = [];
        } catch (err) {
          console.log(err);
          userInputs = [];
        }
      } // console.log('you typed: ' + joined);


      $('#historyTextArea').text(joined);
    };
  } // RenderMarkdown();
  // function RenderMarkdown() {
  //     console.log('rendering markdown');
  //     try {
  //         var text = $('#raw-setup-md').text();
  //         var target = $('#rendered-setup-md');
  //         var converter = new showdown.Converter();
  //         var html = converter.makeHtml(text);
  //         target.html(html);
  //         // $('pre code').each(function(i, e) {hljs.highlightBlock(e)});
  //     }
  //     catch (err) {
  //         console.log(err);
  //     }
  // }

});

function collectBrowserDetails() {
  var nVer = navigator.appVersion;
  var nAgt = navigator.userAgent;
  var browserName = navigator.appName;
  var fullVersion = '' + parseFloat(navigator.appVersion);
  var majorVersion = parseInt(navigator.appVersion, 10);
  var nameOffset, verOffset, ix; // In Opera, the true version is after "Opera" or after "Version"

  if ((verOffset = nAgt.indexOf("Opera")) != -1) {
    browserName = "Opera";
    fullVersion = nAgt.substring(verOffset + 6);
    if ((verOffset = nAgt.indexOf("Version")) != -1) fullVersion = nAgt.substring(verOffset + 8);
  } // In MSIE, the true version is after "MSIE" in userAgent
  else if ((verOffset = nAgt.indexOf("MSIE")) != -1) {
      browserName = "Microsoft Internet Explorer";
      fullVersion = nAgt.substring(verOffset + 5);
    } // In Chrome, the true version is after "Chrome" 
    else if ((verOffset = nAgt.indexOf("Chrome")) != -1) {
        browserName = "Chrome";
        fullVersion = nAgt.substring(verOffset + 7);
      } // In Safari, the true version is after "Safari" or after "Version" 
      else if ((verOffset = nAgt.indexOf("Safari")) != -1) {
          browserName = "Safari";
          fullVersion = nAgt.substring(verOffset + 7);
          if ((verOffset = nAgt.indexOf("Version")) != -1) fullVersion = nAgt.substring(verOffset + 8);
        } // In Firefox, the true version is after "Firefox" 
        else if ((verOffset = nAgt.indexOf("Firefox")) != -1) {
            browserName = "Firefox";
            fullVersion = nAgt.substring(verOffset + 8);
          } // In most other browsers, "name/version" is at the end of userAgent 
          else if ((nameOffset = nAgt.lastIndexOf(' ') + 1) < (verOffset = nAgt.lastIndexOf('/'))) {
              browserName = nAgt.substring(nameOffset, verOffset);
              fullVersion = nAgt.substring(verOffset + 1);

              if (browserName.toLowerCase() == browserName.toUpperCase()) {
                browserName = navigator.appName;
              }
            } // trim the fullVersion string at semicolon/space if present


  if ((ix = fullVersion.indexOf(";")) != -1) fullVersion = fullVersion.substring(0, ix);
  if ((ix = fullVersion.indexOf(" ")) != -1) fullVersion = fullVersion.substring(0, ix);
  majorVersion = parseInt('' + fullVersion, 10);

  if (isNaN(majorVersion)) {
    fullVersion = '' + parseFloat(navigator.appVersion);
    majorVersion = parseInt(navigator.appVersion, 10);
  }

  var browserInfo = {
    Name: browserName,
    FullVersion: fullVersion,
    MajorVersion: majorVersion,
    NavigatorAppName: navigator.appName,
    NavigatorUserAgent: navigator.userAgent
  };
  return browserInfo;
}

function invokeCSCode(data, source) {
  if (!(typeof invokeCSharpAction === "function")) {
    alert('Looks like the invokeCSharpAction method is not defined 😨. Not to worry, I guess I\'ll let you off for viewing outside of the sample app 😉');
    return;
  }

  try {
    var sourceNative = $(source)[0];

    try {
      var elementCoords = {
        X: sourceNative.getBoundingClientRect().left,
        Y: sourceNative.getBoundingClientRect().top
      };
    } catch (_unused) {
      var elementCoords = {
        X: null,
        Y: null
      };
    }

    try {
      var screenSize = {
        X: Math.max(document.documentElement.clientWidth || 0, window.innerWidth || 0),
        Y: Math.max(document.documentElement.clientHeight || 0, window.innerHeight || 0)
      };
    } catch (_unused2) {
      var screenSize = {
        X: null,
        Y: null
      };
    }

    var sourceId = $(source).attr('id');
    var elementId = sourceId != null ? sourceId : "Unknown";
    invokeCSharpAction(data, elementCoords, screenSize, collectBrowserDetails(), elementId);
  } catch (err) {
    console.log(err);
    alert('An error occurred invoking c# action: ' + err);
  }
}

function invokeCSharpAction(data, elementCoords, screenSize, browserDetails, elementId) {
  var browserInvocation = {
    BrowserUrl: window.location.href,
    Data: data,
    ElementCoordinates: elementCoords,
    DisplayDimensions: screenSize,
    BrowserInfo: browserDetails,
    ElementName: elementId
  };
  var json = JSON.stringify(browserInvocation);
  console.log(json);
}