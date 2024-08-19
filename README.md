# HeirOfLegend
![image](https://github.com/user-attachments/assets/191012f0-a567-4c9a-8d23-e057cab5847a)

# 목차

| 프로젝트 소개 |
| :---: |
| **사용 기술 스택** |
| **기술적 고민과 트러블 슈팅** |
| **만든 사람들** |

# 프로젝트 소개

### 🎎[Team Notion](https://www.notion.so/teamsparta/619f97d922e8454bb7be43ffdf34d62d)

| **게임명**       | 전설의 계승자                 |
|:---:|:---:|
| **장르**         | 2D 탑다운 로그라이트 |
| **개발 환경**    | Unity 2022.3.17f1 |
| **타겟 플랫폼**  | PC / Web |
| **개발 기간**    | 2024.06.27 ~ 2024.08.22        |

# 사용 기술 스택

# 기술적 고민과 트러블 슈팅
- 피격 효과 FlashWhite
    
    문제 발생 : 캐릭터 피격 효과를 흰색으로 표현하고 싶어서
    sprite의 색상을 흰색으로 조절하였으나 흰색은 표현X
    
    원인 파악 : Sprites-Default Material은 argb ( 알파, 빨강, 초록,
    녹색 ) 이상은 어떤 효과도 지원해주지 않는 모양
    
    문제 해결 : 흰색으로 표현해주는 Shader추가 및 해당 Shader가
    적용된 Material생성 후 피격 순간 잠시 Material을
    교체하는 방식으로 적용하여 해결
    
    ![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/83c75a39-3aba-4ba4-a792-7aefe4b07895/e5c7d03c-0e9e-4e01-b37c-8710ab073c92/image.png)
    
    ![image.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/83c75a39-3aba-4ba4-a792-7aefe4b07895/1f0e722d-0f4d-4526-97d2-62524accad0a/image.png)

  - 데미지 표시
    
    
    | 문제 발생 : | 캐릭터 피격시 데미지 값이 y축 상승하며 사라지는것을 구현하려 하였으나
    상승 및 감마값 변화가 적용되지 않는 문제 발생 |
    | --- | --- |
    | 원인 파악 | elapsedTime += Time.deltaTime;
    Vector3.Lerp(startPosition, endPosition, elapsedTime);로 
    구현을 하였으나 Time.deltaTime으로 하면 동작이 되질 않는것을 파악 |
    | 문제 해결 | elapsedTime = 0f;  duration = 2f; 
    elapsedTime += Time.deltaTime; 시작과 끝을 정하고
    증가하는 시작시간에 끝을 나눈 값을 대입 하여 해결 |
    
    ![화면 캡처 2024-08-14 160215.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/83c75a39-3aba-4ba4-a792-7aefe4b07895/df29d5b3-0a2b-450f-bc60-13d422a6bfb1/%ED%99%94%EB%A9%B4_%EC%BA%A1%EC%B2%98_2024-08-14_160215.png)
    
    ![화면 캡처 2024-08-16 145850.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/83c75a39-3aba-4ba4-a792-7aefe4b07895/c028aa02-0e05-475f-b4d6-c12bae48c49f/%ED%99%94%EB%A9%B4_%EC%BA%A1%EC%B2%98_2024-08-16_145850.png)

- 아이템 습득 시 보유 개수값 처리 오류
    
    **`문제 발생`**:  **`ItemData`** SO파일에 현재 중첩개수를 저장할 변수값 (**`currentStack`**)을 추가한 후, 아이템 획득 시 획득 수량이 다른 모든 슬롯의 아이템 개수에 적용되는 버그 발견
    
    **`문제 원인`**: 인벤토리 슬롯의 아이템의 현재 보유개수를 저장하는 과정을 간편화하기 위해 **`ItemData`** SO파일에 현재 보유개수를 저장하는 변수값(**`currentStack`**)을 추가했고, 특정 인벤토리 슬롯의 보유개수를 확인했을 때 모든 아이템 슬롯이 **`currentStack`**을 참조하게 됨.  그래서 첫번째 칸에도 n개가 있으면은 두번째 칸에도 n개가 있다고 뜨는 문제가 생김. 스크립터블 오브젝트에 대한 이해도가 부족해서 발생한 오류
    
    **`문제 해결`**:  **`ItemData`** SO파일에서 보유개수 저장값(**`currentStack`**)을 일단 제거하고, 각 인벤토리 슬롯이 아이템 데이터를 가지고 있는 **`ItemPrefab`** 스크립트 컴포넌트를 지닌 오브젝트 프리팹을 항상 가지고 있게 구조를 변경함. 이후 **`ItemPrefab`** 스크립트 내부에 현재 보유 개수를 저장하는 값(**quantity**)을 사용하기로 결정함.
    

- 랜덤 맵 생성하는 과정에서 겹치는 오류
    
    **`문제 발생` :**  프리펩해놓은 맵들을 랜덤하게 호출 하면서 서로의 방이 겹치면서 생성이 되는것을 발견
    
    **`문제 원인` : GenerateRooms** 을 실행하여 프리펩 해놓은 맵들을 랜덤하게 호출을 해오지만 서로의 크기와 거리를 생각하지 않고 생성을 하게 되어있어 맵들이 호출되면서 겹치게 되었다 맵을 생성하면서 서로의 거리를 생각하지 않고 스크립트를 작성한것이 문제가되었음
    
    **`문제 해결` : GenerateRooms** 함수가 실행이 될 때 타일멥의 방의 크기를 계산하고 서로의 거리를 gridSize를 통해 새로운 방의 위치를 겹치지 않게 조정하였습니다.



# 만든 사람들
| 이름   | 담당                           | 블로그 주소                           | 깃허브 주소                               |
|--------|--------------------------------|----------------------------------------|-------------------------------------------|
| 손민욱 | **팀장**, *Player*, *Enemy*            | [lilduby.tistory.com](https://lilduby.tistory.com/) | [github.com/LilDuby](https://github.com/LilDuby) |
| 정창영 | **부팀장**                         | [bfcat.tistory.com](https://bfcat.tistory.com/) | [github.com/bfcat46](https://github.com/bfcat46) |
| 이정호 | **팀원**                           | [velog.io/@leejungho/posts](https://velog.io/@leejungho/posts) | [github.com/roekdk](https://github.com/roekdk) |
| 곽송우 | **팀원**, *아이템*, *인벤토리*         | [velog.io/@winner2280/posts](https://velog.io/@winner2280/posts) | [github.com/TunnelSight](https://github.com/TunnelSight) |
