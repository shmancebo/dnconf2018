function Buscar()
{
    var url = "http://dotnetapisearch.azurewebsites.net/api/search?index=csindex&query=" + $("#queryinput").val();
    $.get(url, function (data)
    {
        var result = data;
    });

}