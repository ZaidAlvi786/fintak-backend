const fs = require("fs");
let allLabels = [];
function readFiles(dirname, onFileContent, onError) {
  fs.readdir(dirname, function (err, filenames) {
    if (err) {
      onError(err);
      return;
    }
    filenames.forEach(function (filename) {
      fs.readFile(dirname + filename, "utf-8", function (err, content) {
        if (err) {
          onError(err);
          return;
        }
        onFileContent(filename, content);
      });
    });
  });
}

var data = {};
readFiles(
  "./",
  function (filename, content) {
    if (filename.indexOf("_word") === -1) {
      extractDics(content, filename);
      createFile();
    }
  },
  function (err) {
    throw err;
  }
);

function extractDics(content, filename) {
  const matches = content.match(/TranslateHelper.get\("(.)+"\)/gm);
  if (matches) {
    console.log(matches);
    for (let i = 0; i < matches.length; i++) {
      allLabels.push(
        matches[i].replace('TranslateHelper.get("', "").replace('")', "")
      );
    }
  } else {
    console.log("matches not found ", filename);
  }
}

function createFile() {
  const uniqueLabels = [...new Set(allLabels)];
  fs.appendFile("./_words.csv", uniqueLabels.join("\n"), (err) => {
    if (err) {
      console.log(err);
    } else {
      // done
    }
  });
}

<!-- Auto-push timestamp: 2026-04-19 22:21:51 -->