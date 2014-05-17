nbits = 16;
nchans = 1;
Fs_TX = 22050;
Fs_RX = 22050;
ID = 0;

time = zeros(25);

for j=1:25
    r = audiorecorder(Fs_RX, nbits, nchans, ID);

    x = zeros(22050,1);
    x(1) = 1;
    
    p = audioplayer(x, Fs_TX);
    
    %Recording
    record(r);
    play(p);
    
    pause(1);
    stop(r);
    y = getaudiodata(r);

    max_val = 0;
    for i = 100:length(y)
        if abs(y(i)) > max_val
            max_val = abs(y(i));
            time(j) = i * 1000 / Fs_RX;
        end
    end
    disp(time(j));
    
    plot(linspace(0,1,length(y)), y);
    
    pause(1);
end

time_mean = mean(time)
time_std = std(time)