# InternetSpeedTest
A simple command-line tool for measuring your internet download speed, written in C# and tested on Debian.

## Prerequisites
- .NET Core Runtime (version 3.1 or later)

## Usage
To run the program, open a terminal or command prompt, navigate to the directory containing the compiled executable, and run the following command:

```bash
dotnet  InternetSpeedTest.dll [options]
```

### Options
- `--verbose`: Enable verbose mode, which prints additional information during the test.

- `--dfile <url>`: Specify the URL of the test file to download. If not provided, the default URL `http://speedtest.ftp.otenet.gr/files/test100Mb.db` will be used.

## Example
To run the speed test with verbose output and a custom test file URL:

```bash
dotnet  InternetSpeedTest.dll  --verbose  --dfile  https://example.com/test.file
```

This command will run the internet speed test in verbose mode, downloading the test file from `https://example.com/test.file`.

## How it Works
The program downloads a test file from the specified URL (or the default URL if not provided) and measures the download speed by calculating the running mean speed based on the number of bytes read and the elapsed time. It displays the running mean download speed in Mbps and a progress bar during the download.

After the download is complete, the final running mean download speed is displayed.

## License
This project is licensed under the [MIT License](LICENSE).