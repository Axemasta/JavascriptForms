// To include a HTML file use this code
// <script> include('path/to/file.html', document.currentScript) </script>

function include(file, element) {
    req(file)
        .then(code => insertCode(code, element))
        .catch(() => showError())
}

function insertCode(code, referenceElem) {
    const { tags: scripts, codeWithoutTags: htmlAndCSS } = getTags(code, 'script')
    const { tags: styles, codeWithoutTags: html } = getTags(htmlAndCSS, 'style')

    referenceElem.insertAdjacentHTML('beforebegin', html)   // HTML
    styles.forEach(style => insertTag(style, 'style'))      // Styles
    scripts.forEach(script => insertTag(script, 'script'))  // Scripts
}

function showError() {
    console.log('Error getting file')
}

function insertTag(code, tagName) {
    const referenceElem = document.querySelector(`[data-${tagName}-tag]`)

    let elem = document.createElement(tagName)
    elem.textContent = code
    referenceElem.parentNode.insertBefore(elem, referenceElem.previousSibling)

    referenceElem.remove()
}
function getTags(code, tagName) {
    const patternForGettingTag = new RegExp(`(?<=<${tagName}>)(.*?)(?=<\/${tagName}>)`, 'gs')
    const patternForRemovingTag = new RegExp(`<${tagName}>(.*?)\/${tagName}>`, 'gs')

    const tags = code.match(patternForGettingTag)
    const codeWithoutTags = code.replace(patternForRemovingTag, `<${tagName} data-${tagName}-tag></${tagName}>`)

    return {
        'tags': tags === null ? [] : tags,
        'codeWithoutTags': codeWithoutTags
    }
}

// Requesting file
function req(fileName) {
    return new Promise((res, rej) => {
        var req = new XMLHttpRequest()

        req.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status != 200)
                    return rej()

                res(this.responseText)
            }
        }

        req.open("GET", fileName, true)
        req.send()
    })
}