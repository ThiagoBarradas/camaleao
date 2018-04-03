var _request = undefined;
	
	  var NextPath = function(content){
      var indice = content.indexOf('.');
      return content.substring(indice+1, content.length);;
    }

    function GetElement(path){
      
      var result;
      function go(elements, path){
          var pathSplit = path.split('.');
          
          if(Array.isArray(elements)){
              var size = elements.length - 1;
          
              while(size >= 0 && result === undefined){
                if(pathSplit.length > 1){
                  result = go(elements[size][pathSplit[0]], NextPath(path));
                }else{
                  result = elements[size][pathSplit[0]];
                }
                size--;
              }
          }else if(pathSplit.length > 1){
              go(elements[pathSplit[0]], NextPath(path));
          }else{
              result = elements[pathSplit[0]]
          }
          
          return result;
      }
      
		return go(_request, path);
	}
	
	function GetComplexElement(path){
		return JSON.stringify(GetElement(path));
	}
	
	function Contains(path, search){
		var element = GetElement(path);
		if(element != undefined && element === search){
			return true;
		}
		return false;
	}
	
	function ContainsIgnoreCase(path, search){
		var element = GetElement(path);
		if(element != undefined && element.toUpperCase() === search.toUpperCase()){
			return true;
		}
		return false;
	}
	
	function NotContains(path, search){
		return !Contains(path, search);
	}
	
	function NotContainsIgnoreCase(path, search){
		return !ContainsIgnoreCase(path, search);
    }

    function GetDateTime() {
        var d = new Date();
        function pad(n) { return n < 10 ? '0' + n : n }
        return d.getUTCFullYear() + '-'
            + pad(d.getUTCMonth() + 1) + '-'
            + pad(d.getUTCDate()) + 'T'
            + pad(d.getUTCHours()) + ':'
            + pad(d.getUTCMinutes()) + ':'
            + pad(d.getUTCSeconds()) + 'Z'
    }

    function GetRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }