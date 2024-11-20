mergeInto(LibraryManager.library, {
  OpenFolderPicker: function (callbackObjectNamePtr, callbackFunctionNamePtr, callbackFunctionSuccessPtr , onPreLoadCallbackFunctionNamePtr){
    var callbackObjectName = UTF8ToString(callbackObjectNamePtr);
    var callbackFunctionName = UTF8ToString(callbackFunctionNamePtr);
    var callbackFunctionSuccess = UTF8ToString(callbackFunctionSuccessPtr);
    var onPreLoadCallbackFunctionName = UTF8ToString(onPreLoadCallbackFunctionNamePtr);

    var input = document.createElement('input');
    input.type = 'file';
    input.webkitdirectory = true;
    input.multiple = true;

    input.onchange = function (event) {
      var files = event.target.files;
      var filesData = [];
      var filesProcessed = 0;

      var processFile = function(file) {
        var reader = new FileReader();
        reader.onload = function(e) {
          // Remove the data URL prefix to get only the base64 content
          var base64Data = e.target.result.split(',')[1];
          
          var data = {
            name: file.webkitRelativePath,
            data: base64Data
          };
          SendMessage(callbackObjectName, callbackFunctionName, JSON.stringify(data));
          filesProcessed++;
          if (filesProcessed === files.length) {
            var json = JSON.stringify(filesData);
            SendMessage(callbackObjectName, callbackFunctionSuccess, "done");
          }
        };
        reader.readAsDataURL(file);
      };

      SendMessage(callbackObjectName, onPreLoadCallbackFunctionName, "upload files"); 
      for (var i = 0; i < files.length; i++) {
        processFile(files[i]);
      }
    };

    input.click();
  },
  OpenFilePicker: function (callbackObjectNamePtr, callbackFunctionNamePtr, callbackFunctionSuccessPtr , onPreLoadCallbackFunctionNamePtr){
    var callbackObjectName = UTF8ToString(callbackObjectNamePtr);
    var callbackFunctionName = UTF8ToString(callbackFunctionNamePtr);
    var callbackFunctionSuccess = UTF8ToString(callbackFunctionSuccessPtr);
    var onPreLoadCallbackFunctionName = UTF8ToString(onPreLoadCallbackFunctionNamePtr);
    var input = document.createElement('input');
    input.type = 'file';
    input.accept = '.zip';
    input.onchange = function (event) {
      var file = event.target.files[0];
      var reader = new FileReader();
      var filesData = [];

      reader.onload = function () {
        JSZip.loadAsync(reader.result).then(function (zip) {
          SendMessage(callbackObjectName, onPreLoadCallbackFunctionName, "unzip file");

          var promises = [];

          zip.forEach(function (relativePath, zipEntry) {
            var promise =  zipEntry.async('base64').then(function (content) {
              console.log("read file",relativePath);
              var data = {
                name: relativePath,
                data: content
              };
              SendMessage(callbackObjectName, callbackFunctionName, JSON.stringify(data));
            });
            promises.push(promise);
          });
          Promise.all(promises).then(function () {
            var json = JSON.stringify(filesData);
            console.log("all data", json);
            SendMessage(callbackObjectName, callbackFunctionSuccess, "done");
          });
        });
      };
      reader.readAsArrayBuffer(file);
    };
    input.click();
  },
});


