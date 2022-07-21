$( document ).ready(function() {
  $('.create_org').on('click', function() {
      $('.modal-wrapper-create').toggleClass('open');
      return false;
  });
  $('.join_an_org').on('click', function() {
      $('.modal-wrapper-join').toggleClass('open');
      return false;
  });
  $('.head').on('click', function (){
      $('.modal-wrapper-create').removeClass('open');
	  $('.modal-wrapper-join').removeClass('open');
  })
});