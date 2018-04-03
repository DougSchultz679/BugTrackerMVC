var input = 'Turpentine and turtles.';
const vowels = ['A','E','I','O','U'];
var resultArray = [];

function whaleTranslator(){
    input = input.toUpperCase();
for (i=0;i<input.length;i++){
  /*if (input.charAt(i)===' '){
      resultArray.push(input.charAt(i));}
 else*/ if (input.charAt(i)==='.'){
      resultArray.push('!!! ');}
  else if (input.charAt(i)==='E'){
    resultArray.push('EE');}
  else if (input.charAt(i)==='U'){
    resultArray.push('UU');}
  else{
  for (j=0;j<vowels.length;j++){
    if (input.charAt(i) === vowels[j]){
      resultArray.push(vowels[j]);}}
	}
}
  console.log(resultArray.join(''));
}
whaleTranslator();