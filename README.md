It is this telegram boat for creating car insurance policy documents for customer. 

Pipeline to set up the bot for developers. 

You need :
1. Mindee account with Api keys for parsing "International Id(EU)" and "License plate (EU)"
2. IronPdfApiKey (lib for pdf creating)
3. OpenAiApiKey which has GPT_4o in available models
4. Telegram bot with its token.

You can host it on local machine or with cloud.
Anyway, set the environment variables first with your tokens: 
{
        "IronPdfApiKey": "_",
        "MindeeApiKey1": "_", (for api which can process InternationalIdV1)
        "MindeeApiKey2": "_", (for api which can process LicensePlateV1)
        "OpenAiApiKey": "_",
        "BotToken": "_"
}
Also you can change the code in the MindeePictureProcessor to use only 1 mindeeApiKey (i jsut used 2 beause of different free accounts)

Now you can run the application and use the bot.
Also you can pugh it into Azure with publishing profiles or host somewhere else (but remember to set environment variables there too).


Pipeline for user :
1. Start dialog.

2. Bot will greet you, describe its purpose to you.
   
3. Bot will ask for passport photo.

4. a. You are sending photo. (next pipline steps are assuming this case)
   b. You are sending anything else. Text, sticker, emoji, video, etc.
   In that case bot will notify you, that your input is incorrect and will ask you again, reminding you your isntructions on this step.
   Let's call it "Incorrect input reaction" for short.

5. a. Third-api could not extract some fields from ypur photo --> Bot will ask you to upload another photo, making accent on visibility of data. --> Step 3.
   b. Bot will extract some data from it using 3rd-party API of computer vision and will ask if extracted data is correct and accurate.
   It will provide you with 2 buttons "Yes" or "No" to answer the question.

6. a. You press "yes" --> Step 7.
   b. You press "no" --> Bot will ask you to upload another photo, making accent on visibility of data. --> Step 3.
   c. You send anything to the bot. --> "Incorrect input reaction"

7. Bot will ask you to upload you license plate photo. Proccess will be same ass Step 3-6.

8. a. You are sending photo. (next pipline steps are assuming this case)
   b. You are sending anything else. Text, sticker, emoji, video, etc. --> Incorrect input reaction

9. a. Third-api could not extract some fields from ypur photo --> Bot will ask you to upload another photo, making accent on visibility of data. --> Step 7.
   b. Bot will extract some data from it using 3rd-party API of computer vision and will ask if extracted data is correct and accurate.
   It will provide you with 2 buttons "Yes" or "No" to answer the question.

10. a. You press "yes" --> Step 6.
   b. You press "no" --> Bot will ask you to upload another photo, making accent on visibility of data. --> Step 7.
   c. You send anything to the bot. --> "Incorrect input reaction"

11. Bot will announce the price of creating insurance policy to you. Provinding you buttons to agree or disagree.

12. a. You disagree with price by clicking "No". Bot will say sorry to hear that and will end the process. Any next message will be treated as step 1.
    b. You send anything to bot --> "Incorrect input handle"
    c. You agreed to the price by clicking "Yes" --> step 13

13. Bot will send you your insurance policy document as pdf, with your passport and license plate data. Return to step 1.
