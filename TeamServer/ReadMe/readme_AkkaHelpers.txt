표준화 목적
창의와 혁신은 새로운 방법의 발견이며 그것은 곧 새로운 표준의 발견이다.

가치
- 설계는 구현이다.
- 설계 시작과 끝은 테스트다.
- 테스트는 요구사항 명세서다.

버전
2.1.1.0


목차
1. ActorPaths 매뉴얼
2. 버전 이력

[1. ActorPaths 매뉴얼]
1. ActorPaths 클래스 내용을 수정한다(또는 ActorPaths.cs 클래스 파일을 만든다).
	예.
	// typeof을 사용한 버전(기본)
    public static class ActorPaths
    {
		// 1수준 Actor Path 정의
        public static readonly ActorMetaData ParentActor = new ActorMetaData(typeof(ParentActor).Name);
		
		// 2수준 Actor Path 정의: 부모 Actor를 마지막 Parameter로 전달 시킨다.
        public static readonly ActorMetaData ChildActor = new ActorMetaData(typeof(ChildActor).Name, ParentActor);
    }

    // nameof을 사용한 버전(C# 6 이상, Visual Studio 2015 이상)
    public static class ActorPaths
    {
		// 1수준 Actor Path 정의
        public static readonly ActorMetaData ParentActor = new ActorMetaData(nameof(ParentActor));
		
		// 2수준 Actor Path 정의: 부모 Actor를 마지막 Parameter로 전달 시킨다.
        public static readonly ActorMetaData ChildActor = new ActorMetaData(nameof(ChildActor), ParentActor);
    }

2. Actor 생성 및 Path 접근할 때 ActorPaths을 사용한다.
	using AkkaHelpers;

	system.ActorOf(HelloActor.Pros(), ActorPaths.HelloActor.Name);
	system.ActorSelection(ActorPaths.HelloActor.Path).Tell( ... );
	

[2. 버전 이력]
2.1.1.0
2.1.0.0
1.1.2018.207
1.1.2018.206
1.1.2018.205
1.1.2018.202
1.1.2018.201
1.1.2018.131
1.1.2018.129
1.2.2018.125
1.2.2018.124
1.1.2018.104
1.0.2017.1212
1.0.2017.1211
1.0.2017.1127

[버전 세부 정보 - 2.1.1.0]
- Added
	readme.txt 파일 목차 앞에 버전 추가

[버전 세부 정보 - 2.1.0.0]
- Added
	정식 버전 배포
- Changed
	readme_AkkaHelpers.txt 파일을 "ReadMe" 폴더로 이동

[버전 세부 정보 - 1.1.2018.206]
- Changed
	<code> 주석 보강

[버전 세부 정보 - 1.1.2018.206]
- Added
	<code> 주석 추가
- Fixed
	xUnit, OpenCover 프로세스 절대 경로 제거
	복수개 테스트 프로젝트를 파일 복사 없이 xUnit 실행하여 OpenCover 결과 생성
	.Tests.dll 파일을 obj 폴더까지 찾는 버그 수정
- Changed
	".build/TestResults"을 ".build/CodeCoverage"로 변경
	".build/NuGetPackage"을 ".build/NuGetPackages"로 변경
	readme 예제 Actor 이름 변경
	
[버전 세부 정보 - 1.1.2018.205]
- Added
	클래스와 메소드에 표준 주석(///)을 추가
	AkkaHelpers.xml 파일을 포함한 패키지 배포
- Fixed
	readme에 nameof 키워드 사용 주석 정정

[버전 세부 정보 - 1.1.2018.202]
- Fixed
	'함수'를 '메소드'로 정정

[버전 세부 정보 - 1.1.2018.201]
- Fixed
	단위 테스트 메소드 이름 오타 정정

[버전 세부 정보 - 1.1.2018.131]
- Added
	readme_Mirero.Service.TestKit.txt 가치 추가
- Changed
	모든 단위 테스트 메소드를 '한글'에서 '영어'로 변경

[버전 세부 정보 - 1.1.2018.129]
- Added
	readme_Mirero.Service.TestKit.txt 표준화 목적 추가

[버전 세부 정보 - 1.2.2018.125]
- Removed
	HoconMetaData 클래스 및 단위 테스트 제거

[버전 세부 정보 - 1.2.2018.124]
- Added
	Hocon 접근을 위한 HoconMetaData.cs 파일 추가.
	HoconMetaData 클래스를 테스트하기 위한 HoconMetaData_Should.cs 파일을 추가.
- Changed
	모든 단위 테스트 메소드를 '영어'에서 '한글'로 변경.

[버전 세부 정보 - 1.1.2018.104]
- Removed
	ActorSystem 이름을 위한 변수를 제거.
	ActorPaths.cs 파일 제거
		Why?
		Mirero.Service.TestKit는 ActorPaths.cs 파일이 필요 없지만 자동으로 생성된다.
		자동으로 생성되기 때문에 단위 테스트 대상 프로젝트에 있는 ActorPaths.cs 파일과 겹치는 문제가 발생된다.

[버전 세부 정보 - 1.0.2017.1212]
- Added
	ActorPaths.cs 파일을 컴파일 성공 상태로 자동 생성 시킨다.

[버전 세부 정보 - 1.0.2017.1211]
- Added
	ActorPaths.cs 파일을 자동 생성 시킨다.

[버전 세부 정보 - 1.0.2017.1127]
- Added
	최초 배포
	https://github.com/petabridge/akkadotnet-helpers