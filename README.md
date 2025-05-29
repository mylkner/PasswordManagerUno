first ever app. kinda made it just to learn uno ig. pretty fun platform so far. will probably continue to use. 

anyway, short explanation:
user sets a master password and 2 keys are derived with different salts. one for auth, one for encryption.
master password hash, and both salts are saved in db. on login, input password is hashed using same salt and compared to saved hash.
encryption key is stored on login in a singleton as a byte array and persists throughout app lifecycle.
passwords are encrypted with AES, a unique iv and the encryption hash/key.
the title, the encrypted password, and the unique iv are added to db. on decryption they're retrieved and plain text password is copied to clipboard.

didn't do a crazy amount of research or anything so not sure how good that is. but the point was only to learn uno, not make an industry level app or anything.
overall good fun.
