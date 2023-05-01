/////////////////////////////////////////////
// SHADER LOD LEVEL//////////////////////////
/////////////////////////////////////////////

// # basical tips #
// #define to open keyword.
// #undef  to close keyword.

#if   defined (QUALITY_HIGH)
      #define _NORMALMAP 1
    

#elif defined (QUALITY_MEDIUM) 
      #define _NORMALMAP 1

#elif defined (QUALITY_LOW)
      #define _NORMALMAP 1

#else 
      #define _NORMALMAP 1

#endif