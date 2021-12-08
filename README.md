<center><img src="/photo/icon.JPG" width="25%" height="25%"></center>

# PROLOCK DEFENDER [Fileless 랜섬웨어 PROLOCK 백신]
> 주제 : Fileless 랜섬웨어 Prolock을 탐지하고 방어하는 백신 개발 <br>
> 개발 기간 : 2020.04 ~ 2020.07

## 목차
1. 배경
2. Prolock의 공격 실행 과정
3. 탐지 기술 소개
4. 방어 기술 소개
5. 동영상 시연

## 1. 배경
- 2019년 12월 Prolock의 부모인 페이로드 전체가 쉘코드인 Fileless 랜섬웨어 PwndLocker가 등장합니다.
- 2020년 2월 PwndLocker에서 암호화 알고리즘을 패치하여 상위버전인 ProLock으로 진화합니다.
- 해외에서 Prolock으로 인한 실제 피해사례가 발생했습니다.
- 그러나 국내에서는 Prolock을 탐지조차 하지 못하는 백신이 존재합니다.
- 이에 Prolock을 탐지하고 방어하는 백신을 개발하고 싶었습니다.
<center><img src="/photo/1.JPG" width="75%" height="75%"></center>

## 2. Prolock의 공격 실행 과정
- Prolock은 4개의 파일로 존재합니다. 
<center><img src="/photo/2.JPG" width="75%" height="75%"></center>

- 각 파일은 다음 그림과 같은 순으로 실행됩니다.
- run.bat 파일이 실행되면 Winmgr.xml 파일을 작업 스케줄러에 등록하고 실행됩니다. 실행되는 순간 run.bat 파일과 Winmgr.xml 파일을 삭제시킵니다.
- Winmgr.bat 파일이 실행되면 clean.bat 파일이 실행됩니다.
- clean.bat 파일이 실행되면 PowerShell을 실행시키고 Winmgr.bmp 파일에서 바이너리 코드를 추출하고 메모리로 로드합니다.
- Winmgr.bmp 파일에서 추출된 바이너리 코드는 쉘코드를 구성하고 이는 랜섬웨어를 동작시킵니다.
<center><img src="/photo/3.JPG" width="75%" height="75%"></center>

## 3. 탐지 기술 소개
- 현재 Prolock의 공격이 실행 중인 파워쉘을 특정합니다.
- Prolock이 수행되는 파워쉘은 cmd를 부모로 갖고, 이 cmd는 svchost.exe를 부모로 갖습니다.
<center><img src="/photo/4.JPG" width="75%" height="75%"></center>

- Prolock은 세 가지 API와 한 가지 함수를 사용합니다.
- 파워쉘에 입력되는 명령어 로그를 실시간 모니터링하여 Prolock의 공격이 시작되는 순간을 확인합니다.
<center><img src="/photo/5.JPG" width="75%" height="75%"></center>

## 4. 방어 기술 소개
- 이전에 특정했던 파워쉘을 강제 종료하여 파워쉘을 차단합니다.
<center><img src="/photo/6.JPG" width="75%" height="75%"></center>

- Prolock의 주요 페이로드가 시작되면 메모리 사용량이 순간적으로 증폭됩니다. 모니터링하여 이 순간은 확인합니다.
- Prolock은 Fileless 특성을 유지하기 위해 자체적으로 Prolock과 관련된 모든 파일을 삭제합니다.
- Prolock은 모든 파일을 삭제 후 본격적인 랜섬웨어 공격을 시작합니다.
- 모든 파일 삭제 시점과 공격 시점 중간에 파워쉘을 차단하여 랜섬웨어 공격을 방어합니다.
- Prolock의 특성을 통해 Prolock 방어에 성공합니다.
<center><img src="/photo/7.JPG" width="75%" height="75%"></center>

## 5. 동영상 시연

[![IU(아이유) _ Into the I-LAND](http://img.youtube.com/vi/1mYRIV9o5-4/0.jpg)](https://youtu.be/1mYRIV9o5-4) 
