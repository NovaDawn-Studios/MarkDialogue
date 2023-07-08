```md
> John walks up to Pete

JOHN
How are you today?

PETE
Good, thanks! How about you?

#if isTrue("JohnWasShot") 

  %% Note: isTrue is a potential method implemented by the game, not MarkDialogue. %%

  JOHN (Pained)
  I'm doing alright, my knee is healing, although walking is still a pain.

#else

  JOHN
  Doing fine, glad I dodged that shot!

#endif

PETE (Happy)
Glad to hear. Hey, have you heard about Don Lenzo?

> At that moment, the Don walks in

CRIMEBOSS
You ain't talkin' 'bout me, are ya lads?

PETE (Worried)
Oh, no sir!
Well...

#if visited(Talked about stolen money)
  CRIMEBOSS (Dangerous)
  So you know about the cash, eh?
#else
  CRIMEBOSS
  You have nothing on me, kid. Shut ya trap!
#endif
```