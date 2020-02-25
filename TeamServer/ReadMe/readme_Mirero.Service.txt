표준화 목적
창의와 혁신은 새로운 방법의 발견이며 그것은 곧 새로운 표준의 발견이다.

가치
- 설계는 구현이다.
- 설계 시작과 끝은 테스트다.
- 테스트는 요구사항 명세서다.

버전
2.1.1.0


목차
1. 패키지 업데이트 및 추가
2. 파일 구성
3. 기본 매뉴얼
4. Powershell 4.0 설치 매뉴얼
5. Petabridge.Cmd.Host 설치 매뉴얼
6. Petabridge.Cmd.Host 사용자 매뉴얼
7. 버전 이력

[1. 패키지 업데이트 및 추가]
1. 기본 설치된 패키지 업데이트를 확인한다.
	Akka
	NLog
	Topshelf
	...

2. 추가 설치할 패키지를 설치한다.
	Akka.Remote				// TCP/IP
	Dapper					// ORM
	RunProcessAsTask		// Process(Exe) 실행
	...


[2. 파일 구성]
1. ActorService
	ActorServiceConfigurator.cs		: Topshelf 서비스 관리
	ActorServiceCommandLine.cs		: 서비스 커맨드 라인 매개변수
	HoconPaths.cs					: App.config HOCON 경로
	ActorServiceHost.cs				: 서비스 시작/중지 이벤트 처리, 액터 시스템 관리
	UserSupervisorStrategy.cs		: /user 장애 처리 전략
2. Actors
	HeloActor.cs					: 예제 액터
3. App
	ActorPaths.cs					: 액터 계층 구조
	Program.cs						: 프로세스 시작
	App.config						: 서비스 및 액터 시스템 환경 설정


[3. 기본 매뉴얼]
1. Mirero.Service 설치 확인하기
	1.1 디버그 모드로 컴파일한다.

	1.2 디버깅(F5)으로 실행 한다.
		Petabridge.Cmd 활성화를 위해 원격 접속 허용 대화상자에서 "확인" 버튼을 클릭한다.

	1.3 UserSupervisorStrategy 생성자에 설정한 "Breakpoint" 호출 유/무를 확인한다.
		호출되면 설정이 정상적으로 변경된 것이다.
	
	1.4 실행된 결과를 확인한다.
		서비스 실행 결과(1초 간격으로 반복 출력한다): "Hello! MireroSystem."

	1.5 로그 파일 생성을 확인한다.
		[실행파일 폴더]\log\[프로제트 이름].log

	1.6(선택적) Petabridge.Cmd Port을 변경한다.
		(복수개 Petabridge.Cmd을 실행 시키기 위해서는 Port을 구분해야 한다.)
		App.config 파일에 "port = 9110" 기본 값이 지정되어 있다.


[4. Powershell 4.0 설치 매뉴얼]
1. PowerShell을 실행 시킨다.
2. $PSVersionTable.PSVersion을 입력하여 실행 시킨다.
	PS C:\Users\홍길동> $PSVersionTable.PSVersion
		Major  Minor  Build  Revision
		-----  -----  -----  --------
		5      1      16299  98
3. 3.0이하 버전일 때는 4.0 이상으로 설치해야 한다.
		
4. PowerShell 4.0 URL(Windows Management Framework 4.0)
	https://www.microsoft.com/ko-KR/download/details.aspx?id=40855
	
5. 	설치 파일 다운로드 받기
	x64일 때
	Windows6.1-KB2819745-x64-MultiPkg.msu
	
	x86일 때
	Windows6.1-KB2819745-x86-MultiPkg.msu


[5. Petabridge.Cmd.Host 설치 매뉴얼]
1. 관리자 권한으로 콘솔 창(Cmd 창)을 연다.

2. Choco 설치를 위해 아래 명령을 복사하여 실행 시킨다(현재 폴더 위치는 상관 없다).
@"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -InputFormat None -ExecutionPolicy Bypass -Command "iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"

3. Choco 설치가 정상적으로 끝나면 아래 명령을 실행하여 확인한다.
	C:\>choco
		Chocolatey v0.10.8
		Please run 'choco -?' or 'choco <command> -?' for help menu.

4. Petabridge-Cmd 설치를 위해 콘솔 창에 아래 명령을 복사하여 실행 시킨다(0.3.2 버전일 때 아래 문구를 확인할 수 있다).
	C:\>choco install petabridge-cmd

		Chocolatey v0.10.8
		Installing the following packages:
		petabridge-cmd
		By installing you accept licenses for the packages.
		Progress: Downloading petabridge-cmd 0.3.2... 100%

		petabridge-cmd v0.3.2 [Approved]
		petabridge-cmd package files install completed. Performing other installation steps.
		 ShimGen has successfully created a shim for pbm.exe
		 The install of petabridge-cmd was successful.
		  Software install location not explicitly set, could be in package or
		  default install location if installer.

		Chocolatey installed 1/1 packages.
		 See the log for details (C:\ProgramData\chocolatey\logs\chocolatey.log).

5. Petabridge-Cmd 설치가 정상적으로 끝나면 아래 명령을 실행하여 확인한다.
	C:\>pbm
		pbm 0.3.2.0

		Usage: pbm [host:port]

		Options:
			pbm help    Show help

		Instructions:

		pbm will automatically connect to the provided node address and download the list of availabile
		instructions supported by the server for this particular host.

		The default address for pbm is [127.0.0.1:9110], but check with your system administrator.


[6. Petabridge.Cmd.Host 사용자 매뉴얼]
1. 서비스를 DEBUG으로 실행 시킨다.
	[INFO][1/17/2018 1:26:07 AM][Thread 0004][[akka://BLUE/user/petabridge.cmd#1312969692]] petabridge.cmd host bound to [0.0.0.0:9110]

2. 서비스에 접속하기
	2.1 접속할 때
	C:\>pbm 127.0.0.1:9110
			successfully connected to [::ffff:127.0.0.1]:9110
			Commands downloaded from server. type `help` to see what's available

	2.2 접속된 상태에서 Actor System 이름 확인하기
		[127.0.0.1:9110] pbm> actor system
			akka://BLUE

	2.3 접속된 상태에서 Actor 계층 구조 확인하기
		[127.0.0.1:9110] pbm> actor hierarchy
			/user
			/user/HelloActor
			/user/pbm-uptime
			/user/petabridge.cmd
			/user/petabridge.cmd/127.0.0.1%3A51451
			/user/petabridge.cmd/127.0.0.1%3A51451/actor
			/user/petabridge.cmd-log-memorizer

	2.4 접속 해제 확인하기
		[127.0.0.1:9110] pbm>
			connection to host terminated.
	
	2.5 주요 명령어 정리
		actor system
		actor hierarchy -d 1		// 0은 /user다.
		actor hierarchy -d 6 > actor-hierarchy.txt
		actor hierarchy -s /user/foo/child1 -d 1


[7. 버전 이력]
2.1.1.0
2.1.0.0
1.1.2018.221
1.1.2018.202
1.1.2018.201
1.1.2018.131
1.1.2018.129
1.1.2018.128
1.1.2018.117
1.1.2018.110
1.1.2018.109
1.1.2018.108
1.1.2018.104
1.0.2017.1211

[버전 세부 정보 - 2.1.1.0]
- Fixed
	현상: Linux에서 NLog가 App.config 파일을 읽지 못함
	해결: NLog 버전을 4.0.0.0에서 4.4.13으로 변경
- Added
	NLog 기본 설정으로 throwExceptions="true" 추가
	readme.txt 파일 목차 앞에 버전 추가
- Changed
	AkkaHelpers 2.1.0에서 2.1.1로 변경
	App.config 파일을 테스트하기 위해 ActorServiceConfigurator 클래스에 ReadConfigurationFromFile 정적 메소드로 변경
		단위 테스트 예.
		public Router_Should() : base(ActorServiceConfigurator.ReadConfigurationFromFile())
	"ReadMe > Mirero.Service > Manual > App" 폴더에 있는 모든 *.cs.txt 파일을 "App" 폴더를 제거하여 상위 폴더로 이동 시킴

[버전 세부 정보 - 2.1.0.0]
- Added
	정식 버전 배포
	"네임스페이스" 변경을 자동화 시킴
	Coding Convention 추가
	Manual 추가(클래스 파일 설명 txt 파일)
- Changed
	readme_Mirero.Service.txt 파일을 "ReadMe" 폴더로 이동
	코드에 있던 주석을 분리 시킴(xxx.cs.txt)
- Removed
	readme_Mirero.Service.txt 파일에서 "네임스페이스" 변경 제거

[버전 세부 정보 - 1.1.2018.221]
- Added
	전용(Local) 메시지와 전용(Remote, Cluster) 메시지 구분
	전용(Remote, Cluster) 메시지 예제 추가
- Fixed
	build_nuget.cake 파일에 누락된 의존성 추가

[버전 세부 정보 - 1.1.2018.202]
- Added
	공용/전용 메시지 클래스 구성 주석 추가
	Powershell 설치 매뉴얼 readme에 추가

[버전 세부 정보 - 1.1.2018.201]
- Added
	App.config(HOCON)에 있는 서비스 옵션 정보 접근 방법 예제 추가
- Changed
	단위 테스트 메소드 이름을 출력 중심으로 변경

[버전 세부 정보 - 1.1.2018.131]
- Added
	readme 파일에 표준화 목적과 가치 추가
- Changed
	주석 보강
	단위 테스트 메소드를 모두 "한글"에서 "영어"로 변경
- Removed
	GettingStarted 문서와 예제등 관련 자료를 모두 "사내 Bootcamp"로 이동

[버전 세부 정보 - 1.1.2018.129]
- Added
	RouterActor 코딩 컨벤션

[버전 세부 정보 - 1.1.2018.128]
- Added
	Mirero.Service_설계서.pdf
	ActorServcie 폴더 밑으로 Mirero.Service 클래스들 추가
		ActorServiceConfigurator
		ActorServiceCommandLine	
		HoconPaths				
		ActorServiceHost		
		UserSupervisorStrategy	
	GettingStarted_Service.mp4
- Removed
	"[3. Getting Started 예제 매뉴얼]"을 readme에서 제거(Getting Started 예제에서 제공한다)

[버전 세부 정보 - 1.1.2018.110]
- Changed
	"[메뉴얼]"을 액터 생성까지 반영

[버전 세부 정보 - 1.1.2018.109]
- Added
	Service Message-Driven Architecture 설계 표준화.pdf	

[버전 세부 정보 - 1.1.2018.108]
- Added 
	환경 파일
		App.config
	소스 파일
		Program.cs
		ActorPaths.cs
		ActorService.cs
		HelloActor.cs
		UserSupervisorStrategy.cs

[버전 세부 정보 - 1.1.2018.104]
- Changed
	AkkaHelpers 2018.1.3.1 => AkkaHelpers 1.1.2018.104

[버전 세부 정보 - 2018.01.03.01]
- Changed
	AkkaHelpers 1.0.2017.1211 => AkkaHelpers 2018.1.3.1
	
[버전 세부 정보 - 1.0.2017.1127]
- Added
	최초 배포
	Akka 1.3.2
	Akka.Logger.NLog 1.2.0
	AkkaHelpers 1.0.2017.1211
	Topshelf 3.3.1
	Topshelf.NLog 3.3.1