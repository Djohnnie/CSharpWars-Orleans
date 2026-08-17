mergeInto(LibraryManager.library, {
    CSharpWars_GetApiBaseAddress: function () {
        var config = window.csharpWarsConfig;
        if (!config || typeof config.apiBaseAddress !== "string") {
            return 0;
        }

        var bufferSize = lengthBytesUTF8(config.apiBaseAddress) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(config.apiBaseAddress, buffer, bufferSize);
        return buffer;
    }
});
