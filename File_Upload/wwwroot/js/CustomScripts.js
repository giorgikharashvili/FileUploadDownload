function downloadFile(fileDataUrl)
{
    fetch(fileDataUrl).then(response => response.blob()).then(blob => {
        // Creates 'a' link like in HTML for example: <a>My Link </a>
        var link = window.document.createElement("a");
        link.href = window.URL.createObjectURL(blob);
        link.download = "download_ " + new Date().toISOString().slice(0, 10);

        //So whats this does is we create a <a href="LinkLoaction">LinkName</a> in HTML dinamiclly and then we click it
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    })
}