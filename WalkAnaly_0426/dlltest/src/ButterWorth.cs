/*
* MATLAB Compiler: 4.18.1 (R2013a)
* Date: Fri Mar 31 01:01:17 2017
* Arguments: "-B" "macro_default" "-W" "dotnet:dlltest,ButterWorth,4.0,private" "-T"
* "link:lib" "-d" "D:\產學資料\WalkAnaly\dlltest\src" "-w"
* "enable:specified_file_mismatch" "-w" "enable:repeated_file" "-w"
* "enable:switch_ignored" "-w" "enable:missing_lib_sentinel" "-w" "enable:demo_license"
* "-v" "class{ButterWorth:D:\產學資料\WalkAnaly\myfunc.m}" 
*/
using System;
using System.Reflection;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;

#if SHARED
[assembly: System.Reflection.AssemblyKeyFile(@"")]
#endif

namespace dlltest
{

  /// <summary>
  /// The ButterWorth class provides a CLS compliant, MWArray interface to the MATLAB
  /// functions contained in the files:
  /// <newpara></newpara>
  /// D:\產學資料\WalkAnaly\myfunc.m
  /// <newpara></newpara>
  /// deployprint.m
  /// <newpara></newpara>
  /// printdlg.m
  /// </summary>
  /// <remarks>
  /// @Version 4.0
  /// </remarks>
  public class ButterWorth : IDisposable
  {
    #region Constructors

    /// <summary internal= "true">
    /// The static constructor instantiates and initializes the MATLAB Compiler Runtime
    /// instance.
    /// </summary>
    static ButterWorth()
    {
      if (MWMCR.MCRAppInitialized)
      {
        try
        {
          Assembly assembly= Assembly.GetExecutingAssembly();

          string ctfFilePath= assembly.Location;

          int lastDelimiter= ctfFilePath.LastIndexOf(@"\");

          ctfFilePath= ctfFilePath.Remove(lastDelimiter, (ctfFilePath.Length - lastDelimiter));

          string ctfFileName = "dlltest.ctf";

          Stream embeddedCtfStream = null;

          String[] resourceStrings = assembly.GetManifestResourceNames();

          foreach (String name in resourceStrings)
          {
            if (name.Contains(ctfFileName))
            {
              embeddedCtfStream = assembly.GetManifestResourceStream(name);
              break;
            }
          }
          mcr= new MWMCR("",
                         ctfFilePath, embeddedCtfStream, true);
        }
        catch(Exception ex)
        {
          ex_ = new Exception("MWArray assembly failed to be initialized", ex);
        }
      }
      else
      {
        ex_ = new ApplicationException("MWArray assembly could not be initialized");
      }
    }


    /// <summary>
    /// Constructs a new instance of the ButterWorth class.
    /// </summary>
    public ButterWorth()
    {
      if(ex_ != null)
      {
        throw ex_;
      }
    }


    #endregion Constructors

    #region Finalize

    /// <summary internal= "true">
    /// Class destructor called by the CLR garbage collector.
    /// </summary>
    ~ButterWorth()
    {
      Dispose(false);
    }


    /// <summary>
    /// Frees the native resources associated with this object
    /// </summary>
    public void Dispose()
    {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary internal= "true">
    /// Internal dispose function
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        disposed= true;

        if (disposing)
        {
          // Free managed resources;
        }

        // Free native resources
      }
    }


    #endregion Finalize

    #region Methods

    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the myfunc MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// ====== 巴特沃斯濾波器 ======
    /// [b, a] = butter(filterOrder, [1/(50/2) 2/(50/2)]);    低通1/(50/2)，高通2/(50/2)
    /// [b, a] = butter(filterOrder, [15/(120/2)], 'low');
    /// y = filter(b, a, data);
    /// c = data;
    /// d = y;
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray myfunc()
    {
      return mcr.EvaluateFunction("myfunc", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the myfunc MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// ====== 巴特沃斯濾波器 ======
    /// [b, a] = butter(filterOrder, [1/(50/2) 2/(50/2)]);    低通1/(50/2)，高通2/(50/2)
    /// [b, a] = butter(filterOrder, [15/(120/2)], 'low');
    /// y = filter(b, a, data);
    /// c = data;
    /// d = y;
    /// </remarks>
    /// <param name="infilename">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray myfunc(MWArray infilename)
    {
      return mcr.EvaluateFunction("myfunc", infilename);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the myfunc MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// ====== 巴特沃斯濾波器 ======
    /// [b, a] = butter(filterOrder, [1/(50/2) 2/(50/2)]);    低通1/(50/2)，高通2/(50/2)
    /// [b, a] = butter(filterOrder, [15/(120/2)], 'low');
    /// y = filter(b, a, data);
    /// c = data;
    /// d = y;
    /// </remarks>
    /// <param name="infilename">Input argument #1</param>
    /// <param name="outfilename">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray myfunc(MWArray infilename, MWArray outfilename)
    {
      return mcr.EvaluateFunction("myfunc", infilename, outfilename);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the myfunc MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// ====== 巴特沃斯濾波器 ======
    /// [b, a] = butter(filterOrder, [1/(50/2) 2/(50/2)]);    低通1/(50/2)，高通2/(50/2)
    /// [b, a] = butter(filterOrder, [15/(120/2)], 'low');
    /// y = filter(b, a, data);
    /// c = data;
    /// d = y;
    /// </remarks>
    /// <param name="infilename">Input argument #1</param>
    /// <param name="outfilename">Input argument #2</param>
    /// <param name="filterOrder">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray myfunc(MWArray infilename, MWArray outfilename, MWArray filterOrder)
    {
      return mcr.EvaluateFunction("myfunc", infilename, outfilename, filterOrder);
    }


    /// <summary>
    /// Provides a single output, 4-input MWArrayinterface to the myfunc MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// ====== 巴特沃斯濾波器 ======
    /// [b, a] = butter(filterOrder, [1/(50/2) 2/(50/2)]);    低通1/(50/2)，高通2/(50/2)
    /// [b, a] = butter(filterOrder, [15/(120/2)], 'low');
    /// y = filter(b, a, data);
    /// c = data;
    /// d = y;
    /// </remarks>
    /// <param name="infilename">Input argument #1</param>
    /// <param name="outfilename">Input argument #2</param>
    /// <param name="filterOrder">Input argument #3</param>
    /// <param name="Lawpass">Input argument #4</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray myfunc(MWArray infilename, MWArray outfilename, MWArray filterOrder, 
                    MWArray Lawpass)
    {
      return mcr.EvaluateFunction("myfunc", infilename, outfilename, filterOrder, Lawpass);
    }


    /// <summary>
    /// Provides a single output, 5-input MWArrayinterface to the myfunc MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// ====== 巴特沃斯濾波器 ======
    /// [b, a] = butter(filterOrder, [1/(50/2) 2/(50/2)]);    低通1/(50/2)，高通2/(50/2)
    /// [b, a] = butter(filterOrder, [15/(120/2)], 'low');
    /// y = filter(b, a, data);
    /// c = data;
    /// d = y;
    /// </remarks>
    /// <param name="infilename">Input argument #1</param>
    /// <param name="outfilename">Input argument #2</param>
    /// <param name="filterOrder">Input argument #3</param>
    /// <param name="Lawpass">Input argument #4</param>
    /// <param name="Highpass">Input argument #5</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray myfunc(MWArray infilename, MWArray outfilename, MWArray filterOrder, 
                    MWArray Lawpass, MWArray Highpass)
    {
      return mcr.EvaluateFunction("myfunc", infilename, outfilename, filterOrder, Lawpass, Highpass);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the myfunc MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// ====== 巴特沃斯濾波器 ======
    /// [b, a] = butter(filterOrder, [1/(50/2) 2/(50/2)]);    低通1/(50/2)，高通2/(50/2)
    /// [b, a] = butter(filterOrder, [15/(120/2)], 'low');
    /// y = filter(b, a, data);
    /// c = data;
    /// d = y;
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] myfunc(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "myfunc", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the myfunc MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// ====== 巴特沃斯濾波器 ======
    /// [b, a] = butter(filterOrder, [1/(50/2) 2/(50/2)]);    低通1/(50/2)，高通2/(50/2)
    /// [b, a] = butter(filterOrder, [15/(120/2)], 'low');
    /// y = filter(b, a, data);
    /// c = data;
    /// d = y;
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="infilename">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] myfunc(int numArgsOut, MWArray infilename)
    {
      return mcr.EvaluateFunction(numArgsOut, "myfunc", infilename);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the myfunc MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// ====== 巴特沃斯濾波器 ======
    /// [b, a] = butter(filterOrder, [1/(50/2) 2/(50/2)]);    低通1/(50/2)，高通2/(50/2)
    /// [b, a] = butter(filterOrder, [15/(120/2)], 'low');
    /// y = filter(b, a, data);
    /// c = data;
    /// d = y;
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="infilename">Input argument #1</param>
    /// <param name="outfilename">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] myfunc(int numArgsOut, MWArray infilename, MWArray outfilename)
    {
      return mcr.EvaluateFunction(numArgsOut, "myfunc", infilename, outfilename);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the myfunc MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// ====== 巴特沃斯濾波器 ======
    /// [b, a] = butter(filterOrder, [1/(50/2) 2/(50/2)]);    低通1/(50/2)，高通2/(50/2)
    /// [b, a] = butter(filterOrder, [15/(120/2)], 'low');
    /// y = filter(b, a, data);
    /// c = data;
    /// d = y;
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="infilename">Input argument #1</param>
    /// <param name="outfilename">Input argument #2</param>
    /// <param name="filterOrder">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] myfunc(int numArgsOut, MWArray infilename, MWArray outfilename, 
                      MWArray filterOrder)
    {
      return mcr.EvaluateFunction(numArgsOut, "myfunc", infilename, outfilename, filterOrder);
    }


    /// <summary>
    /// Provides the standard 4-input MWArray interface to the myfunc MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// ====== 巴特沃斯濾波器 ======
    /// [b, a] = butter(filterOrder, [1/(50/2) 2/(50/2)]);    低通1/(50/2)，高通2/(50/2)
    /// [b, a] = butter(filterOrder, [15/(120/2)], 'low');
    /// y = filter(b, a, data);
    /// c = data;
    /// d = y;
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="infilename">Input argument #1</param>
    /// <param name="outfilename">Input argument #2</param>
    /// <param name="filterOrder">Input argument #3</param>
    /// <param name="Lawpass">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] myfunc(int numArgsOut, MWArray infilename, MWArray outfilename, 
                      MWArray filterOrder, MWArray Lawpass)
    {
      return mcr.EvaluateFunction(numArgsOut, "myfunc", infilename, outfilename, filterOrder, Lawpass);
    }


    /// <summary>
    /// Provides the standard 5-input MWArray interface to the myfunc MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// ====== 巴特沃斯濾波器 ======
    /// [b, a] = butter(filterOrder, [1/(50/2) 2/(50/2)]);    低通1/(50/2)，高通2/(50/2)
    /// [b, a] = butter(filterOrder, [15/(120/2)], 'low');
    /// y = filter(b, a, data);
    /// c = data;
    /// d = y;
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="infilename">Input argument #1</param>
    /// <param name="outfilename">Input argument #2</param>
    /// <param name="filterOrder">Input argument #3</param>
    /// <param name="Lawpass">Input argument #4</param>
    /// <param name="Highpass">Input argument #5</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] myfunc(int numArgsOut, MWArray infilename, MWArray outfilename, 
                      MWArray filterOrder, MWArray Lawpass, MWArray Highpass)
    {
      return mcr.EvaluateFunction(numArgsOut, "myfunc", infilename, outfilename, filterOrder, Lawpass, Highpass);
    }


    /// <summary>
    /// Provides an interface for the myfunc function in which the input and output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// ====== 巴特沃斯濾波器 ======
    /// [b, a] = butter(filterOrder, [1/(50/2) 2/(50/2)]);    低通1/(50/2)，高通2/(50/2)
    /// [b, a] = butter(filterOrder, [15/(120/2)], 'low');
    /// y = filter(b, a, data);
    /// c = data;
    /// d = y;
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void myfunc(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("myfunc", numArgsOut, ref argsOut, argsIn);
    }



    /// <summary>
    /// This method will cause a MATLAB figure window to behave as a modal dialog box.
    /// The method will not return until all the figure windows associated with this
    /// component have been closed.
    /// </summary>
    /// <remarks>
    /// An application should only call this method when required to keep the
    /// MATLAB figure window from disappearing.  Other techniques, such as calling
    /// Console.ReadLine() from the application should be considered where
    /// possible.</remarks>
    ///
    public void WaitForFiguresToDie()
    {
      mcr.WaitForFiguresToDie();
    }



    #endregion Methods

    #region Class Members

    private static MWMCR mcr= null;

    private static Exception ex_= null;

    private bool disposed= false;

    #endregion Class Members
  }
}
